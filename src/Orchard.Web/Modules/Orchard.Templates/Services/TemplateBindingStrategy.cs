using System;
using System.Web;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Mvc.Spooling;

namespace Orchard.Templates.Services {
    public class TemplateBindingStrategy : IShapeTableProvider {
        private readonly IWorkContextAccessor _wca;

        public TemplateBindingStrategy(IWorkContextAccessor wca) {
            _wca = wca;
        }

        public void Discover(ShapeTableBuilder builder) {
            ExecuteInWorkContext(context => BuildShapes(builder, context.Resolve<ITemplateService>(), context.Resolve<ITemplateCache>()));
        }

        private void BuildShapes(ShapeTableBuilder builder, ITemplateService templatesService, ITemplateCache cache) {
            var shapes = templatesService.GetTemplates();
            foreach (var record in shapes) {
                cache.Set(record.Name, record.Template);

                var shapeType = AdjustName(record.Name);
                builder.Describe(shapeType)
                       .BoundAs("Template::" + shapeType,
                                descriptor => context => {
                                    var template = cache.Get(record.Name);
                                    return template != null ? PerformInvoke(context, record.Language, template) : new HtmlString("");
                                });
            }
        }

        private IHtmlString PerformInvoke(DisplayContext displayContext, string type, string template) {
            var service = _wca.GetContext().Resolve<ITemplateService>();
            var output = new HtmlStringWriter();

            if (String.IsNullOrEmpty(template))
                return null;

            output.Write(CoerceHtmlString(service.Execute(template, type, displayContext, displayContext.ViewDataContainer.ViewData.Model)));

            return output;
        }

        private string AdjustName(string name) {
            var lastDash = name.LastIndexOf('-');
            var lastDot = name.LastIndexOf('.');
            if (lastDot <= 0 || lastDot < lastDash) {
                name = Adjust(name, null);
                return name;
            }

            var displayType = name.Substring(lastDot + 1);
            name = Adjust(name.Substring(0, lastDot), displayType);
            return name;
        }

        private static string Adjust(string name, string displayType) {
            // Canonical shape type names must not have - or . to be compatible with display and shape api calls.
            var shapeType = name.Replace("--", "__").Replace("-", "__").Replace('.', '_');

            if (String.IsNullOrEmpty(displayType)) {
                return shapeType;
            }
            var firstBreakingSeparator = shapeType.IndexOf("__", StringComparison.OrdinalIgnoreCase);
            if (firstBreakingSeparator <= 0) {
                return (shapeType + "_" + displayType);
            }

            return (shapeType.Substring(0, firstBreakingSeparator) + "_" + displayType + shapeType.Substring(firstBreakingSeparator));
        }

        private static IHtmlString CoerceHtmlString(object invoke) {
            return invoke as IHtmlString ?? (invoke != null ? new HtmlString(invoke.ToString()) : null);
        }

        private void ExecuteInWorkContext(Action<WorkContext> action) {
            var workContext = _wca.GetContext();
            if (workContext != null) {
                action(workContext);
            }
            else {
                using (var scope = _wca.CreateWorkContextScope()) {
                    action(scope.WorkContext);
                }
            }
        }
    }
}
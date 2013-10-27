using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using Orchard.Mvc.Spooling;

namespace Orchard.Templates.Services {
    public class TemplateBindingStrategy : IShapeTableProvider {
        private readonly IWorkContextAccessor _wca;
        private readonly RouteCollection _routeCollection;

        public TemplateBindingStrategy(
            IWorkContextAccessor wca, 
            RouteCollection routeCollection) {
            _wca = wca;
            _routeCollection = routeCollection;
        }

        public void Discover(ShapeTableBuilder builder) {

            var workContext = _wca.GetContext();
            if (workContext != null)
            {
                BuildShapes(builder, workContext.Resolve<ITemplateService>());
            }
            else
            {
                using (var scope = _wca.CreateWorkContextScope())
                {
                    BuildShapes(builder, scope.Resolve<ITemplateService>());
                }
            }
        }

        private void BuildShapes(ShapeTableBuilder builder, ITemplateService templatesService) {
            var shapes = templatesService.GetTemplates();
            foreach (var record in shapes)
            {
                var shapeType = AdjustName(record.Name);
                builder.Describe(shapeType)
                       .BoundAs("Template::" + shapeType,
                                descriptor => context => PerformInvoke(context, descriptor, record.Language, record.Template));
            }
        }

        private IHtmlString PerformInvoke(DisplayContext displayContext, ShapeDescriptor descriptor, string type, string template)
        {
            var service = _wca.GetContext().Resolve<ITemplateService>();
            var output = new HtmlStringWriter();

            if (String.IsNullOrEmpty(template))
                return null;

            output.Write(CoerceHtmlString(service.Parse(template, type, obj => Activate(obj, displayContext, output), displayContext.ViewDataContainer.ViewData.Model)));

            return output;
        }

        private string AdjustName(string name)
        {
            var lastDash = name.LastIndexOf('-');
            var lastDot = name.LastIndexOf('.');
            if (lastDot <= 0 || lastDot < lastDash)
            {
                name = Adjust(name, null);
                return name;
            }

            var displayType = name.Substring(lastDot + 1);
            name = Adjust(name.Substring(0, lastDot), displayType);
            return name;
        }

        private static string Adjust(string name, string displayType)
        {
            // canonical shape type names must not have - or . to be compatible 
            // with display and shape api calls)))
            var shapeType = name.Replace("--", "__").Replace("-", "__").Replace('.', '_');

            if (string.IsNullOrEmpty(displayType))
            {
                return shapeType.ToLowerInvariant();
            }
            var firstBreakingSeparator = shapeType.IndexOf("__", StringComparison.OrdinalIgnoreCase);
            if (firstBreakingSeparator <= 0)
            {
                return (shapeType + "_" + displayType).ToLowerInvariant();
            }

            return (shapeType.Substring(0, firstBreakingSeparator) + "_" + displayType + shapeType.Substring(firstBreakingSeparator)).ToLowerInvariant();
        }

        private void Activate<TModel>(ITemplateViewBase<TModel> obj, DisplayContext displayContext, TextWriter output)
        {
            obj.Writer = new HtmlTextWriter(output);
            obj.Url = new UrlHelper(displayContext.ViewContext.RequestContext, _routeCollection);
            obj.Html = new HtmlHelper<TModel>(displayContext.ViewContext, displayContext.ViewDataContainer, _routeCollection);
            obj.Ajax = new AjaxHelper<TModel>(displayContext.ViewContext, displayContext.ViewDataContainer, _routeCollection);
            obj.ViewContext = displayContext.ViewContext;
            obj.InitHelpers();
        }

        private static IHtmlString CoerceHtmlString(object invoke)
        {
            return invoke as IHtmlString ?? (invoke != null ? new HtmlString(invoke.ToString()) : null);
        }
    }
}
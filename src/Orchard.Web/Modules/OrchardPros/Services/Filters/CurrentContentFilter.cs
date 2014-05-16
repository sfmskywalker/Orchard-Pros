using System.Web.Mvc;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Mvc.Filters;
using Orchard.UI.PageClass;
using Orchard.Utility.Extensions;
using OrchardPros.Services.Content;

namespace OrchardPros.Services.Filters {
    public class CurrentContentFilter : FilterProvider, IActionFilter {
        private readonly ICurrentContentAccessor _currentContentAccessor;
        private readonly IPageClassBuilder _pageClassBuilder;

        public CurrentContentFilter(ICurrentContentAccessor currentContentAccessor, IPageClassBuilder pageClassBuilder) {
            _currentContentAccessor = currentContentAccessor;
            _pageClassBuilder = pageClassBuilder;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
            var currentContentItem = _currentContentAccessor.CurrentContentItem;
            var autoroutePart = currentContentItem != null ? currentContentItem.As<AutoroutePart>() : default(AutoroutePart);

            if (autoroutePart == null)
                return;

            var slug = autoroutePart.DisplayAlias;
            var contentType = currentContentItem.ContentType.HtmlClassify();
            _pageClassBuilder.AddClassNames(contentType, slug);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {
        }
    }
}
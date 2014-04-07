using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Services;

namespace OrchardPros.Helpers {
    public static class FlavoredStringHelper {
        /// <summary>
        /// Transforms the specified text with the specified flavor into HTML.
        /// </summary>
        public static string TransformText(string text, string flavor) {
            var context = HttpContext.Current.Request.RequestContext.GetWorkContext();
            var filters = context.Resolve<IEnumerable<IHtmlFilter>>();
            return filters.Where(x => x.GetType().Name.Equals(flavor + "filter", StringComparison.OrdinalIgnoreCase)).Aggregate(text, (t, filter) => filter.ProcessContent(t, flavor));
        }
    }
}
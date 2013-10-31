using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.WebPages;

namespace Orchard.Templates.Services {
    public abstract class RazorTemplate : Mvc.ViewEngines.Razor.WebViewPage<dynamic>, ITemplateBase
    {
        public WebPageContext WebPageContext { get; set; }

        public string GetContent() {
            var buffer = new StringBuilder(1024);
            using (var writer = new StringWriter(buffer))
            {
                PushContext(WebPageContext, new HtmlTextWriter(writer));
                Execute();
                PopContext();
            }
            return buffer.ToString();
        }
    }
}
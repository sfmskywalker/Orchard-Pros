using System.IO;
using System.Web.Mvc;
using System.Web.UI;

namespace Orchard.Templates.Services {
    public interface ITemplateBase {
        dynamic Model { get; }
        ViewDataDictionary ViewData { get; set; }
        ViewContext ViewContext { get; set; }
        AjaxHelper<dynamic> Ajax { get; set; }
        HtmlHelper<dynamic> Html { get; set; }
        UrlHelper Url { get; set; }
        void InitHelpers();
    }
}
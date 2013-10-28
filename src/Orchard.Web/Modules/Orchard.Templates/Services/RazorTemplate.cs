using System.Text;
using System.Web.UI;

namespace Orchard.Templates.Services {
    public class RazorTemplate : Mvc.ViewPage<dynamic>, ITemplateBase {
        private StringBuilder _buffer;

        public virtual void Execute(){}

        protected void Write(object value) {
            WriteLiteral(value);
        }

        protected void WriteLiteral(object value) {
            _buffer.Append(value);
        }

        public string GetContent() {
            _buffer = new StringBuilder(1024);
            Execute();
            return _buffer.ToString();
        }

        public new dynamic Model { get; set; }

        HtmlTextWriter ITemplateBase.Writer { get { return Writer; } set { base.Render(value);} }
    }
}
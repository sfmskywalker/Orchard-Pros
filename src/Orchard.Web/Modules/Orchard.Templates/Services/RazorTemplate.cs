using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.WebPages;
using System.Web.WebPages.Instrumentation;

namespace Orchard.Templates.Services {
    public class RazorTemplate : Mvc.ViewPage<dynamic>, ITemplateBase {
        public virtual void Execute(){}

        protected void Write(object value) {
            WriteLiteral(value);
        }

        protected void WriteLiteral(object value) {
            Writer.Write(value);
        }

        public string GetContent() {
            var buffer = new StringBuilder(1024);
            using (var writer = new StringWriter(buffer))
            {
                Writer = new HtmlTextWriter(writer);
                Execute();
                Writer = null;
            }
            return buffer.ToString();
        }

        public new HtmlTextWriter Writer { get; set; }

        dynamic ITemplateBase.Model {
            get { return Model; }
        }

        public virtual void WriteAttribute(string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values) {
            WriteAttributeTo(Writer, name, prefix, suffix, values);
        }

        public virtual void WriteAttributeTo(TextWriter writer, string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values) {
            var first = true;
            var wroteSomething = false;
            if (values.Length == 0) {
                // Explicitly empty attribute, so write the prefix and suffix
                WritePositionTaggedLiteral(writer, prefix);
                WritePositionTaggedLiteral(writer, suffix);
            }
            else {
                foreach (var attrVal in values) {
                    var val = attrVal.Value;

                    bool? boolVal = null;
                    if (val.Value is bool) {
                        boolVal = (bool)val.Value;
                    }

                    if (val.Value == null || (boolVal != null && !boolVal.Value)) {
                        continue;
                    }
                    var valStr = val.Value as string ?? val.Value.ToString();
                    if (boolVal != null) {
                        Debug.Assert(boolVal.Value);
                        valStr = name;
                    }

                    if (first) {
                        WritePositionTaggedLiteral(writer, prefix);
                        first = false;
                    }
                    else {
                        WritePositionTaggedLiteral(writer, attrVal.Prefix);
                    }

                    if (attrVal.Literal) {
                        WriteLiteralTo(writer, valStr);
                    }
                    else {
                        WriteTo(writer, valStr); // Write value
                    }
                    wroteSomething = true;
                }
                if (wroteSomething) {
                    WritePositionTaggedLiteral(writer, suffix);
                }
            }
        }

        private void WritePositionTaggedLiteral(TextWriter writer, PositionTagged<string> value) {
            WriteLiteralTo(writer, value.Value);
        }

        public virtual void WriteLiteralTo(TextWriter writer, string literal) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (literal == null) return;
            writer.Write(literal);
        }

        public virtual void WriteTo(TextWriter writer, object value) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (value == null) return;

            writer.Write(value );
        }

        public new IDisposable Capture(Action<IHtmlString> callback)
        {
            return new CaptureScope(Writer, callback);
        }

        public new IDisposable Capture(dynamic zone, string position = null)
        {
            return new CaptureScope(Writer, html => {
                zone.Add(html, position);
            });
        }
    }
}
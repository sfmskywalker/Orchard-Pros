using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.WebPages;
using System.Web.WebPages.Instrumentation;

namespace Orchard.Compilation.Razor {
    public abstract class RazorTemplateBase : Mvc.ViewEngines.Razor.WebViewPage<dynamic> {
        public WebPageContext WebPageContext { get; set; }

        public void Render(TextWriter writer) {
            PushContext(WebPageContext, writer);
            OutputStack.Push(writer);
            Execute();
            OutputStack.Pop();
            PopContext();
        }

        //    protected new void Write(object value)
        //    {
        //        WriteLiteral(value);
        //    }

        //    protected new void WriteLiteral(object value)
        //    {
        //        Output.Write(value);
        //    }

        //    public virtual new void WriteAttribute(string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
        //    {
        //        WriteAttributeTo(Output, name, prefix, suffix, values);
        //    }

        //    public virtual new void WriteAttributeTo(TextWriter writer, string name, PositionTagged<string> prefix, PositionTagged<string> suffix, params AttributeValue[] values)
        //    {
        //        var first = true;
        //        var wroteSomething = false;
        //        if (values.Length == 0)
        //        {
        //            // Explicitly empty attribute, so write the prefix and suffix
        //            WritePositionTaggedLiteral(writer, prefix);
        //            WritePositionTaggedLiteral(writer, suffix);
        //        }
        //        else
        //        {
        //            foreach (var attrVal in values)
        //            {
        //                var val = attrVal.Value;

        //                bool? boolVal = null;
        //                if (val.Value is bool)
        //                {
        //                    boolVal = (bool)val.Value;
        //                }

        //                if (val.Value == null || (boolVal != null && !boolVal.Value))
        //                {
        //                    continue;
        //                }
        //                var valStr = val.Value as string ?? val.Value.ToString();
        //                if (boolVal != null)
        //                {
        //                    Debug.Assert(boolVal.Value);
        //                    valStr = name;
        //                }

        //                if (first)
        //                {
        //                    WritePositionTaggedLiteral(writer, prefix);
        //                    first = false;
        //                }
        //                else
        //                {
        //                    WritePositionTaggedLiteral(writer, attrVal.Prefix);
        //                }

        //                if (attrVal.Literal)
        //                {
        //                    WriteLiteralTo(writer, valStr);
        //                }
        //                else
        //                {
        //                    WriteTo(writer, valStr); // Write value
        //                }
        //                wroteSomething = true;
        //            }
        //            if (wroteSomething)
        //            {
        //                WritePositionTaggedLiteral(writer, suffix);
        //            }
        //        }
        //    }

        //    private void WritePositionTaggedLiteral(TextWriter writer, PositionTagged<string> value)
        //    {
        //        WriteLiteralTo(writer, value.Value);
        //    }

        //    public virtual void WriteLiteralTo(TextWriter writer, string literal)
        //    {
        //        if (writer == null)
        //            throw new ArgumentNullException("writer");

        //        if (literal == null) return;
        //        writer.Write(literal);
        //    }

        //    public virtual new void WriteTo(TextWriter writer, object value)
        //    {
        //        if (writer == null)
        //            throw new ArgumentNullException("writer");

        //        if (value == null) return;

        //        writer.Write(value);
        //    }
    }
}
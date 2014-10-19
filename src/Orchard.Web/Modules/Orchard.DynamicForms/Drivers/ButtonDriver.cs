﻿using System.Collections.Generic;
using Orchard.DynamicForms.Elements;
using Orchard.Forms.Services;
using Orchard.Layouts.Framework.Drivers;

namespace Orchard.DynamicForms.Drivers {
    public class ButtonDriver : FormsElementDriver<Button> {
        public ButtonDriver(IFormManager formManager) : base(formManager) { }

        protected override IEnumerable<string> FormNames {
            get { yield return "Button"; }
        }

        protected override void DescribeForm(DescribeContext context) {
            context.Form("Button", factory => {
                var shape = (dynamic)factory;
                var form = shape.Fieldset(
                    Id: "Form",
                    _ButtonText: shape.Textbox(
                        Id: "ButtonText",
                        Name: "ButtonText",
                        Title: "Text",
                        Value: "Submit",
                        Classes: new[] { "text", "medium" },
                        Description: T("The button text.")));

                return form;
            });
        }
    }
}
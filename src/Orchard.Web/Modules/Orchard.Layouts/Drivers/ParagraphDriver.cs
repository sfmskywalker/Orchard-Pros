﻿using Orchard.Layouts.Elements;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.ViewModels;

namespace Orchard.Layouts.Drivers {
    public class ParagraphDriver : ElementDriver<Paragraph> {

        protected override EditorResult OnBuildEditor(Paragraph element, ElementEditorContext context) {
            var viewModel = new ParagraphEditorViewModel {
                Text = element.Content
            };
            var editor = context.ShapeFactory.EditorTemplate(TemplateName: "Elements.Paragraph", Model: viewModel);

            if (context.Updater != null) {
                context.Updater.TryUpdateModel(viewModel, context.Prefix, null, null);
                element.Content = viewModel.Text;
            }
            
            return Editor(context, editor);
        }
    }
}
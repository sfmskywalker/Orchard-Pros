﻿using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Layouts.Framework.Elements;

namespace Orchard.Layouts.Framework.Drivers {
    public class ElementEditorContext {
        public ElementEditorContext() {
            EditorResult = new EditorResult();
        }

        public IElement Element { get; set; }
        public dynamic ShapeFactory { get; set; }
        public IValueProvider ValueProvider { get; set; }
        public IUpdateModel Updater { get; set; }
        public IContent Content { get; set; }
        public string Prefix { get; set; }
        public EditorResult EditorResult { get; set; }
    }
}
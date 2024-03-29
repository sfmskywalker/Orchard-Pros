﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;

namespace OrchardPros.Helpers {
    public static class ContentExtensions {
        public static T Field<T>(this ContentPart part, string fieldName) where T:ContentField {
            return (T) part.Get(typeof (T), fieldName);
        }

        public static T FieldValue<TField, T>(this ContentPart part, string fieldName, Func<TField, T> field) where TField : ContentField {
            var f = part.Field<TField>(fieldName);
            return f != null ? field(f) : default(T);
        }

        public static IEnumerable<T> As<T>(this IEnumerable<IContent> contentItems) where T : IContent {
            return contentItems.Where(x => x.Is<T>()).Select(x => x.As<T>());
        }
    }
}
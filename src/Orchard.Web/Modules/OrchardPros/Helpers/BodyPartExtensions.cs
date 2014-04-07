using System;
using System.Web.Mvc;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Settings;
using OrchardPros.Providers.SummaryGenerators;

namespace OrchardPros.Helpers {
    public static class BodyPartExtensions {
        public static string Summarize(this HtmlHelper htmlHelper, BodyPart part, int boundary = 20, string ellpises = "...") {
            return htmlHelper.Summarize(part.Text.TrimSafe(), GetFlavor(part), SummaryGenerators.PlainTextWords, boundary, ellpises);
        }

        public static string Summarize(this HtmlHelper htmlHelper, BodyPart part, string generatorType, int boundary = 20, string ellpises = "...") {
            return htmlHelper.Summarize(part.Text.TrimSafe(), GetFlavor(part), generatorType, boundary, ellpises);
        }

        public static string TransformedText(this BodyPart part) {
            var flavor = GetFlavor(part);
            return FlavoredStringHelper.TransformText(part.Text.TrimSafe(), flavor);
        }

        private static string GetFlavor(BodyPart part) {
            var typePartSettings = part.Settings.GetModel<BodyTypePartSettings>();
            return typePartSettings != null && !String.IsNullOrWhiteSpace(typePartSettings.Flavor) ? typePartSettings.Flavor : part.PartDefinition.Settings.GetModel<BodyPartSettings>().FlavorDefault;
        }
    }
}
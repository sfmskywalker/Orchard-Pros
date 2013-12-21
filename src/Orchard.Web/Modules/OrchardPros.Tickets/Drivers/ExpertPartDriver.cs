using System.Xml;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Drivers {
    public class ExpertPartDriver : ContentPartDriver<ExpertPart> {
        protected override DriverResult Editor(ExpertPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Expert_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Expert", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ExpertPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(ExpertPart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            partElement.SetAttributeValue("Level", part.Level);
            partElement.SetAttributeValue("ExperiencePoints", part.ExperiencePoints);
            partElement.SetAttributeValue("Rating", part.Rating);
        }

        protected override void Importing(ExpertPart part, ImportContentContext context) {
            context.ImportAttribute(part.PartDefinition.Name, "Level", x => part.Level = XmlConvert.ToInt32(x));
            context.ImportAttribute(part.PartDefinition.Name, "ExperiencePoints", x => part.ExperiencePoints = XmlConvert.ToInt32(x));
            context.ImportAttribute(part.PartDefinition.Name, "Rating", x => part.Rating = XmlConvert.ToInt32(x));
        }
    }
}
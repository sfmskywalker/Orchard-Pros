using System;
using System.Xml;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Membership.Models;

namespace OrchardPros.Membership.Drivers {
    public class UserProfilePartDriver : ContentPartDriver<UserProfilePart> {
        protected override DriverResult Display(UserProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile", () => shapeHelper.Parts_UserProfile());
        }

        protected override DriverResult Editor(UserProfilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/UserProfile", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(UserProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }

        protected override void Exporting(UserProfilePart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            partElement.SetAttributeValue("FirstName", part.FirstName);
            partElement.SetAttributeValue("MiddleName", part.MiddleName);
            partElement.SetAttributeValue("LastName", part.LastName);
            partElement.SetAttributeValue("AvatarType", part.AvatarType);
            partElement.SetAttributeValue("CreatedUtc", part.CreatedUtc);
            partElement.SetAttributeValue("LastLoggedInUtc", part.LastLoggedInUtc);

            if(!String.IsNullOrWhiteSpace(part.Bio))
                partElement.SetValue(part.Bio);
        }

        protected override void Importing(UserProfilePart part, ImportContentContext context) {
            context.ImportAttribute(part.PartDefinition.Name, "FirstName", x => part.FirstName = x);
            context.ImportAttribute(part.PartDefinition.Name, "MiddleName", x => part.MiddleName = x);
            context.ImportAttribute(part.PartDefinition.Name, "LastName", x => part.LastName = x);
            context.ImportAttribute(part.PartDefinition.Name, "AvatarType", x => part.AvatarType = (AvatarType)Enum.Parse(typeof(AvatarType), x));
            context.ImportAttribute(part.PartDefinition.Name, "CreatedUtc", x => part.CreatedUtc = XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.Utc));
            context.ImportAttribute(part.PartDefinition.Name, "LastLoggedInUtc", x => part.LastLoggedInUtc = XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.Utc));
            part.Bio = context.Data.El(part.PartDefinition.Name);
        }
    }
}
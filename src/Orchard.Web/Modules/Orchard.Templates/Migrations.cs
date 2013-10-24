using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Orchard.Templates {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("TemplatePartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Name", c => c.WithLength(100))
                .Column<string>("Body", c => c.Unlimited()));

            ContentDefinitionManager.AlterPartDefinition("TemplatePart", part => part
                .Attachable(false)
                .WithDescription("Turns a type into a template. Typically used just by the Template content type."));

            ContentDefinitionManager.AlterTypeDefinition("Template", type => type
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("TemplatePart")
                .Creatable()
                .Draftable());
            return 1;
        }
    }
}
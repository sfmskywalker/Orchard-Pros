using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Indexing;

namespace OrchardPros.Membership {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .WithField("Avatar", field => field
                    .OfType("MediaLibraryPickerField"))
                .WithDescription("Provides additional information about the user."));

            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("UserProfilePart")
                .Indexed("Users"));

            return 1;
        }
    }
}
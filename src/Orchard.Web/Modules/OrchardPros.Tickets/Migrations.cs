using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace OrchardPros.Tickets {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            
            // Voteable
            SchemaBuilder.CreateTable("Vote", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ReplyId", c => c.NotNull())
                .Column<int>("UserId", c => c.NotNull())
                .Column<int>("Points", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            ContentDefinitionManager.AlterPartDefinition("VotablePart", part => part
                .Attachable()
                .WithDescription("Enables users to vote a content item up or down."));

            // Statistics
            SchemaBuilder.CreateTable("StatisticsPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("ViewCount"));

            ContentDefinitionManager.AlterPartDefinition("StatisticsPart", part => part
                .Attachable()
                .WithDescription("Stores statistical information about your content, such as view count."));

            // Attachment
            ContentDefinitionManager.AlterPartDefinition("AttachmentPart", part => part
                .Attachable(false)
                .WithDescription("Stores information about an attachment."));

            ContentDefinitionManager.AlterTypeDefinition("Attachment", type => type
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("AttachmentPart")
                .Creatable(false)
                .Draftable(false));

            ContentDefinitionManager.AlterPartDefinition("AttachmentsHolderPart", part => part
                .Attachable()
                .WithDescription("Turns your content into a dossier capable of holding attachments."));

            // Ticket
            SchemaBuilder.CreateTable("TicketPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Type", c => c.WithLength(32))
                .Column<decimal>("Bounty")
                .Column<DateTime>("DeadlineUtc")
                .Column<int>("ExperiencePoints")
                .Column<DateTime>("SolvedUtc")
                .Column<int>("AnswerId"));

            ContentDefinitionManager.AlterPartDefinition("TicketPart", part => part
                .WithField("Categories", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Category"))
                .WithField("Tags", field => field
                    .OfType("TaxonomyField")
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", "Tag")
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", "True")
                    .WithSetting("TaxonomyFieldSettings.Autocomplete", "True"))
                .Attachable(false)
                .WithDescription("Stores expert information about a ticket."));

            ContentDefinitionManager.AlterTypeDefinition("Ticket", type => type
                .WithPart("CommonPart", p => p
                    .WithSetting("DateEditorSettings.ShowDateEditor", "true"))
                .WithPart("TitlePart")
                .WithPart("AutoroutePart", builder => builder
                    .WithSetting("AutorouteSettings.AllowCustomPattern", "false")
                    .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                    .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'ID', Pattern: 'tickets/{Content.Id}', Description: 'tickets/123'}]")
                    .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                .WithPart("BodyPart")
                .WithPart("TicketPart")
                .WithPart("AttachmentsHolderPart")
                .WithPart("CommentsPart")
                .WithPart("StatisticsPart")
                .Creatable(false)
                .Draftable());

            // Reply
            ContentDefinitionManager.AlterPartDefinition("ReplyPart", part => part
                .Attachable(false)
                .WithDescription("Turns your content type into a Reply."));

            ContentDefinitionManager.AlterTypeDefinition("Reply", type => type
                .WithPart("CommonPart", part => part
                    .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false")
                    .WithSetting("DateEditorSettings.ShowDateEditor", "false"))
                .WithPart("IdentityPart")
                .WithPart("TitlePart")
                .WithPart("ReplyPart")
                .WithPart("BodyPart")
                .WithPart("VotablePart"));

            // Expert
            ContentDefinitionManager.AlterPartDefinition("ExpertPart", part => part
                .Attachable(false)
                .WithDescription("Stores expert information about a user, such as level and experience points."));

            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("ExpertPart"));

            return 1;
        }
    }
}
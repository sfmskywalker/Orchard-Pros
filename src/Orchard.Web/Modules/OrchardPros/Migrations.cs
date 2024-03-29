﻿using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Indexing;
using OrchardPros.Models;

namespace OrchardPros {
    public class Migrations : DataMigrationImpl {
        private readonly IRepository<Country> _countryRepository;

        public Migrations(IRepository<Country> countryRepository) {
            _countryRepository = countryRepository;
        }

        public int Create() {
            // Country
            SchemaBuilder.CreateTable("Country", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Name", c => c.WithLength(64).NotNull())
                .Column<string>("Code", c => c.WithLength(2)));

            // Transaction
            SchemaBuilder.CreateTable("Transaction", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<string>("Handle", c => c.NotNull().WithLength(64))
                .Column<int>("UserId", c => c.NotNull())
                .Column<string>("ProductName", c => c.WithLength(256))
                .Column<string>("Status", c => c.WithLength(64))
                .Column<decimal>("Amount", c => c.NotNull())
                .Column<string>("Currency", c => c.NotNull().WithLength(3))
                .Column<string>("Context", c => c.Nullable().WithLength(64))
                .Column<DateTime>("CreatedUtc", c => c.NotNull())
                .Column<DateTime>("ChargedUtc", c => c.Nullable())
                .Column<DateTime>("CanceledUtc", c => c.Nullable())
                .Column<DateTime>("DeclinedUtc", c => c.Nullable())
                .Column<string>("Reference", c => c.Nullable().WithLength(256)));

            // Transfer
            SchemaBuilder.CreateTable("Transfer", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("RecipientUserId", c => c.NotNull())
                .Column<string>("Status", c => c.WithLength(64))
                .Column<decimal>("Amount", c => c.NotNull())
                .Column<string>("Currency", c => c.NotNull().WithLength(3))
                .Column<string>("Context", c => c.Nullable().WithLength(64))
                .Column<DateTime>("CreatedUtc", c => c.NotNull())
                .Column<DateTime>("CompletedUtc", c => c.Nullable()));

            // Subscription
            SchemaBuilder.CreateTable("Subscription", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("SubscriptionSourceId", c => c.NotNull())
                .Column<int>("UserId", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            SchemaBuilder.CreateTable("SubscriptionSourcePartRecord", table => table
                .ContentPartRecord());

            ContentDefinitionManager.AlterPartDefinition("SubscriptionSourcePart", part => part
                .Attachable()
                .WithDescription("Turns your content into a source of notifications that users can subscribe to."));

            // Position
            SchemaBuilder.CreateTable("Position", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<string>("CompanyName", c => c.WithLength(64))
                .Column<string>("Title", c => c.WithLength(64))
                .Column<string>("Location", c => c.WithLength(64))
                .Column<int>("PeriodStartYear")
                .Column<int>("PeriodStartMonth")
                .Column<int>("PeriodEndYear")
                .Column<int>("PeriodEndMonth")
                .Column<bool>("IsCurrentPosition", c => c.NotNull())
                .Column<string>("Description", c => c.Unlimited())
                .Column<bool>("IsArchived")
                .Column<DateTime>("ArchivedUtc"));

            // Skill
            SchemaBuilder.CreateTable("Skill", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<string>("Name", c => c.WithLength(64))
                .Column<int>("Rating", c => c.NotNull()));

            // Experience
            SchemaBuilder.CreateTable("Experience", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId")
                .Column<int>("PositionId")
                .Column<string>("Description", c => c.Unlimited())
                .Column<DateTime>("CreatedUtc"));

            // Recommendation
            SchemaBuilder.CreateTable("RecommendationPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("RecommendedUserId"));

            ContentDefinitionManager.AlterPartDefinition("RecommendationPart", part => part
                .Attachable(false)
                .WithDescription("Used for the Recommendation content type"));

            ContentDefinitionManager.AlterTypeDefinition("Recommendation", type => type
                .WithPart("CommonPart")
                .WithPart("IdentityPart")
                .WithPart("BodyPart")
                .WithPart("RecommendationPart")
                .Creatable(false)
                .Draftable(false));

            // Profile
            SchemaBuilder.CreateTable("UserProfilePartRecord", table => table
                .ContentPartRecord()
                .Column<int>("CountryId")
                .Column<int>("ExperiencePoints", c => c.NotNull())
                .Column<int>("ActivityPoints", c => c.NotNull())
                .Column<DateTime>("CreatedUtc"));

            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .WithDescription("Stores professional background information about a user"));

            ContentDefinitionManager.AlterPartDefinition("UserProfilePart", part => part
                .WithField("Avatar", field => field
                    .OfType("MediaLibraryPickerField"))
                .WithField("Wallpaper", field => field
                    .OfType("MediaLibraryPickerField"))
                .WithDescription("Provides additional information about the user."));

            // User
            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("UserProfilePart")
                .Indexed("All", "People"));
            
            // Voting
            SchemaBuilder.CreateTable("Vote", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ContentItemId", c => c.NotNull())
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

            // Reply
            ContentDefinitionManager.AlterPartDefinition("RepliesPart", part => part
                .Attachable()
                .WithDescription("Enables your content item to receive replies."));

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
                .WithPart("BodyPart", p => p.WithSetting("BodyTypePartSettings.Flavor", "markdowndeep"))
                .WithPart("AttachmentsHolderPart")
                .WithPart("VotablePart")
                .Indexed("All", "Tickets"));


            // Ticket
            SchemaBuilder.CreateTable("TicketPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Type", c => c.WithLength(32))
                .Column<decimal>("Bounty")
                .Column<DateTime>("DeadlineUtc")
                .Column<int>("ExperiencePoints")
                .Column<DateTime>("SolvedUtc")
                .Column<int>("SolvedByUserId")
                .Column<int>("AnswerId")
                .Column<string>("Categories", c => c.WithLength(512))
                .Column<string>("Tags", c => c.WithLength(512)));

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
                .WithPart("BodyPart", p => p.WithSetting("BodyTypePartSettings.Flavor", "markdowndeep"))
                .WithPart("TicketPart")
                .WithPart("AttachmentsHolderPart")
                .WithPart("RepliesPart")
                .WithPart("StatisticsPart")
                .WithPart("SubscriptionSourcePart")
                .Creatable(false)
                .Draftable()
                .Indexed("All", "Tickets"));

            // Stripe
            ContentDefinitionManager.AlterPartDefinition("StripeSettingsPart", part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("Site", type => type.WithPart("StripeSettingsPart"));

            CreateCountries();
            return 1;
        }

        private void CreateCountries() {
            _countryRepository.Create(new Country { Name = "Netherlands", Code = "nl"});
            _countryRepository.Create(new Country { Name = "France", Code = "fr" });
            _countryRepository.Create(new Country { Name = "United States", Code = "us" });
            _countryRepository.Create(new Country { Name = "Hungary", Code = "hu" });
            _countryRepository.Create(new Country { Name = "Sweden", Code = "se" });
            _countryRepository.Create(new Country { Name = "Poland", Code = "pl" });
            _countryRepository.Create(new Country { Name = "Belgium", Code = "be" });
            _countryRepository.Create(new Country { Name = "Germany", Code = "de" });
        }
    }
}
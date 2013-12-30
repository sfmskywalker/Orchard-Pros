using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Indexing;

namespace OrchardPros.Careers {
    public class Migrations : DataMigrationImpl {
        public int Create() {
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
                .Column<int>("UserId"));

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
            ContentDefinitionManager.AlterPartDefinition("ProfessionalProfilePart", part => part
                .WithDescription("Stores professional background information about a user"));

            // User
            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("ProfessionalProfilePart")
                .Indexed("Users"));
            return 1;
        }
    }
}
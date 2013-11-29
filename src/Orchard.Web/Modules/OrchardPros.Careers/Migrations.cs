using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Indexing;

namespace OrchardPros.Careers {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("Position", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ProfileId")
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

            SchemaBuilder.CreateTable("Skill", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ProfileId")
                .Column<string>("Name", c => c.WithLength(64))
                .Column<int>("Rating", c => c.NotNull()));

            ContentDefinitionManager.AlterPartDefinition("ProfessionalProfilePart", part => part
                .WithDescription("Stores professional background information about a user"));

            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("ProfessionalProfilePart")
                .Indexed("Users"));
            return 1;
        }
    }
}
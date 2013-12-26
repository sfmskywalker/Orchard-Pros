using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace OrchardPros.Tickets {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("Ticket", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("UserId", c => c.NotNull())
                .Column<string>("Title", c => c.WithLength(256).NotNull())
                .Column<string>("Description", c => c.Unlimited().NotNull())
                .Column<string>("Type", c => c.WithLength(32).NotNull())
                .Column<string>("Tags", c => c.Unlimited())
                .Column<decimal>("Bounty", c => c.Nullable())
                .Column<DateTime>("DeadlineUtc", c => c.NotNull())
                .Column<int>("ExperiencePoints", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull())
                .Column<DateTime>("LastModifiedUtc", c => c.Nullable())
                .Column<DateTime>("SolvedUtc", c => c.Nullable())
                .Column<int>("AnswerId", c => c.Nullable())
                .Column<DateTime>("ArchivedUtc", c => c.Nullable()));

            SchemaBuilder.CreateTable("TicketCategory", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("TicketId")
                .Column<int>("CategoryId"));

            SchemaBuilder.CreateTable("Attachment", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("TicketId", c => c.NotNull())
                .Column<string>("FileName", c => c.WithLength(256))
                .Column<int>("DownloadCount", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            SchemaBuilder.CreateTable("Reply", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("TicketId", c => c.NotNull())
                .Column<int>("ParentReplyId", c => c.Nullable())
                .Column<int>("UserId", c => c.NotNull())
                .Column<string>("Body", c => c.Unlimited())
                .Column<DateTime>("CreatedUtc", c => c.NotNull())
                .Column<int>("Votes", c => c.NotNull()));

            SchemaBuilder.CreateTable("Vote", table => table
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("ReplyId", c => c.NotNull())
                .Column<int>("UserId", c => c.NotNull())
                .Column<int>("Points", c => c.NotNull())
                .Column<DateTime>("CreatedUtc", c => c.NotNull()));

            SchemaBuilder.CreateForeignKey("FK_Attachments_Ticket", "Attachment", new[] {"TicketId"}, "Ticket", new[] {"Id"});
            SchemaBuilder.CreateForeignKey("FK_Replies_Ticket", "Reply", new[] {"TicketId"}, "Ticket", new[] {"Id"});
            SchemaBuilder.CreateForeignKey("FK_Replies_Reply", "Reply", new[] { "ParentReplyId" }, "Reply", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("FK_Votes_Reply", "Vote", new[] { "ReplyId" }, "Reply", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition("ExpertPart", part => part
                .Attachable(false)
                .WithDescription("Stores expert information about a user, such as level and experience points"));

            ContentDefinitionManager.AlterTypeDefinition("User", type => type
                .WithPart("ExpertPart"));

            return 1;
        }
    }
}
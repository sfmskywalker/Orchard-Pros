using System;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Orchard.Messaging.Migrations {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class MessagingQueuingMigrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("MessagePriority", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("Rank", c => c.NotNull())
                .Column<string>("Name", c => c.WithLength(50))
                .Column<string>("DisplayText", c => c.WithLength(50)));

            SchemaBuilder.CreateTable("MessageQueue", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<string>("Name", c => c.WithLength(50))
                .Column<string>("Status", c => c.WithLength(50))
                .Column<int>("UpdateFrequency", c => c.NotNull())
                .Column<int>("TimeSlice", c => c.NotNull())
                .Column<DateTime>("StartedUtc")
                .Column<DateTime>("EndedUtc"));

            SchemaBuilder.CreateTable("QueuedMessage", table => table
                .Column<int>("Id", c => c.Identity().PrimaryKey())
                .Column<int>("QueueId", c => c.NotNull())
                .Column<int>("Priority_Id")
                .Column<string>("ChannelName", c => c.WithLength(50))
                .Column<string>("Recipients", c => c.Unlimited())
                .Column<string>("Subject", c => c.WithLength(2048))
                .Column<string>("Body", c => c.Unlimited())
                .Column<string>("ShapeName", c => c.WithLength(100))
                .Column<string>("PropertyBag", c => c.Unlimited())
                .Column<string>("Status", c => c.WithLength(50))
                .Column<DateTime>("CreatedUtc")
                .Column<DateTime>("StartedUtc")
                .Column<DateTime>("CompletedUtc")
                .Column<string>("Result", c => c.Unlimited()));
            return 1;
        }
    }
}
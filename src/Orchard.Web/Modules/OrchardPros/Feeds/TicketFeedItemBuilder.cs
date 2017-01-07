using System.Linq;
using System.Xml.Linq;
using NHibernate.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Feeds;
using Orchard.Core.Feeds.Models;
using Orchard.Core.Title.Models;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Feeds {
    public class TicketFeedItemBuilder : IFeedItemBuilder {
        public void Populate(FeedContext context) {
            foreach (var feedItem in context.Response.Items.OfType<FeedItem<ContentItem>>().Where(feedItem => context.Format == "rss")) {
                var ticketPart = feedItem.Item.As<TicketPart>();
                if (ticketPart == null)
                    continue;

                var categories = ticketPart.Categories.ToArray();
                var tags = ticketPart.Tags.ToArray();
                var item = feedItem;

                categories.ForEach(x => item.Element.Add(new XElement("category", x.As<TitlePart>().Title)));
                tags.ForEach(x => item.Element.Add(new XElement("category", x.As<TitlePart>().Title)));

                item.Element.Add(new XElement("author", ticketPart.User.UserName));
            }
        }
    }
}
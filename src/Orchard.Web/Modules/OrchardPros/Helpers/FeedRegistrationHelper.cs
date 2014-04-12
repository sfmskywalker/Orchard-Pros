using System.Web.Routing;
using Orchard.Core.Feeds;

namespace OrchardPros.Helpers {
    public static class FeedRegistrationHelper {
        public static void RegisterTicketsFeed(this IFeedManager feedManager) {
            feedManager.Register("Tickets", "rss", new RouteValueDictionary(new { tickets = "rss" }));
        }
    }
}
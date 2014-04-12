using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Feeds;
using Orchard.Core.Feeds.Models;
using Orchard.Core.Feeds.StandardBuilders;
using Orchard.Mvc.Extensions;
using Orchard.Services;
using OrchardPros.Services.Content;

namespace OrchardPros.Feeds {
    [UsedImplicitly]
    public class TicketsFeedQuery : IFeedQueryProvider, IFeedQuery {
        private readonly IContentManager _contentManager;
        private readonly IEnumerable<IHtmlFilter> _htmlFilters;
        private readonly ITicketService _ticketService;

        public TicketsFeedQuery(IContentManager contentManager, IEnumerable<IHtmlFilter> htmlFilters, ITicketService ticketService) {
            _contentManager = contentManager;
            _htmlFilters = htmlFilters;
            _ticketService = ticketService;
        }

        public FeedQueryMatch Match(FeedContext context) {
            var ticketsValue = context.ValueProvider.GetValue("tickets");

            if (ticketsValue == null)
                return null;

            return new FeedQueryMatch { FeedQuery = this, Priority = 1 };
        }

        public void Execute(FeedContext context) {
            var limitValue = context.ValueProvider.GetValue("limit");
            var limit = 20;
            if (limitValue != null)
                limit = (int)limitValue.ConvertTo(typeof(int));

            var link = new XElement("link");
            context.Response.Element.SetElementValue("title", "Tickets");
            context.Response.Element.Add(link);
            context.Response.Element.SetElementValue("description", "Orchard Pros Tickets");

            context.Response.Contextualize(requestContext => {
                var urlHelper = new UrlHelper(requestContext);
                var uriBuilder = new UriBuilder(urlHelper.MakeAbsolute("/")) { Path = urlHelper.RouteUrl(new RouteValueDictionary(new { action = "Index", controller = "Ticket", area = "OrchardPros" })) };
                link.Add(uriBuilder.Uri.OriginalString);
            });

            var tickets = _ticketService.GetTickets(0, limit).ToArray();

            foreach (var ticket in tickets) {
                context.Builder.AddItem(context, ticket.ContentItem);
            }
        }
    }
}
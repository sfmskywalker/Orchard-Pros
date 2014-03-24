using System.Linq;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Security;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.Security;

namespace OrchardPros {
    public class Shapes : IShapeTableProvider {
        private readonly IOpenAuthServices _openAuthServices;
        private readonly Work<ITicketService> _ticketService;
        private readonly Work<IContentManager> _contentManager;

        public Shapes(IOpenAuthServices openAuthServices, Work<ITicketService> ticketService, Work<IContentManager> contentManager) {
            _openAuthServices = openAuthServices;
            _ticketService = ticketService;
            _contentManager = contentManager;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("OAuthLogin").OnDisplaying(displaying => {
                displaying.Shape.Providers = _openAuthServices.GetProviders().ToArray();
            });

            builder.Describe("Tickets__HomePage").OnDisplaying(displaying => {
                var tickets = _ticketService.Value.GetTickets(skip: 0, take: 15);
                displaying.Shape.Tickets = tickets;
            });

            builder.Describe("TopContributor__HomePage").OnDisplaying(displaying => {
                var topContributor = _contentManager.Value.Query<UserProfilePart, UserProfilePartRecord>().OrderByDescending(x => x.ExperiencePoints).List().As<IUser>().FirstOrDefault();
                displaying.Shape.User = topContributor;
            });

            builder.Describe("TagCloud__HomePage").OnDisplaying(displaying => {
                var tags = _ticketService.Value.GetPopularTags().ToArray();
                displaying.Shape.Tags = tags;
            });
        }
    }
}

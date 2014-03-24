using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class ReplyPartHandler : ContentHandler {
        private readonly IUserManager _userManager;

        public ReplyPartHandler(IUserManager userManager) {
            _userManager = userManager;
            OnCreated<ReplyPart>(OnReplyCreated);
        }

        private void OnReplyCreated(CreateContentContext context, ReplyPart part) {
            var user = part.User;
            var ticketOwner = part.ContainingContent.As<TicketPart>().User;
            var xpToAdd = _userManager.CalculateXpWhenReplied(ticketOwner);
            _userManager.AddXp(user, xpToAdd);
            _userManager.AddActivityPoints(user, 5);
        }
    }
}
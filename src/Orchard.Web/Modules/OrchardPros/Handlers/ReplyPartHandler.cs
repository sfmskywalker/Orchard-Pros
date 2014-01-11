using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class ReplyPartHandler : ContentHandler {
        private readonly IUserManager _userManager;

        public ReplyPartHandler(IUserManager userManager) {
            _userManager = userManager;
            OnCreated<ReplyPart>(OnReplyCreated);
        }

        private void OnReplyCreated(CreateContentContext context, ReplyPart part) {
            var user = part.User.As<UserProfilePart>();
            var ticketOwner = part.ContainingContent.As<TicketPart>().User.As<UserProfilePart>();
            var xpToAdd = _userManager.CalculateXpWhenReplied(ticketOwner);
            _userManager.AddXp(user, xpToAdd);
        }
    }
}
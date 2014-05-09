using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Security {
    [UsedImplicitly]
    public class ReplyAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Checking(CheckAccessContext context) {
            if (context.Permission.Name != Permissions.EditOwnReply.Name)
                return;

            if (context.Granted || !context.Content.Is<ReplyPart>())
                return;

            if (HasOwnership(context.User, context.Content)) {
                context.Granted = true;
            }
        }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) { }

        private static bool HasOwnership(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var replyPart = content.As<ReplyPart>();
            return user.Id == replyPart.User.Id;
        }
    }
}
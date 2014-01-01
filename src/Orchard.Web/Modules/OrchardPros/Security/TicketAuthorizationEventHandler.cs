using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Security;

namespace OrchardPros.Security {
    [UsedImplicitly]
    public class TicketAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Checking(CheckAccessContext context) {
            if (!context.Granted && context.Content.Is<ICommonPart>()) {
                if (HasOwnership(context.User, context.Content)) {
                    context.Granted = true;
                }
            }
        }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) {}

        private static bool HasOwnership(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var common = content.As<ICommonPart>();
            if (common == null || common.Owner == null)
                return false;

            return user.Id == common.Owner.Id;
        }
    }
}
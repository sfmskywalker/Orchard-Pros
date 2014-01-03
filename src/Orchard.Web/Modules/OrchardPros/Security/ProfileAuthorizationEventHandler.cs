using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Security;

namespace OrchardPros.Security {
    [UsedImplicitly]
    public class ProfileAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Checking(CheckAccessContext context) {
            if (!context.Granted) {
                if (IsCurrentUser(context.User, context.Content)) {
                    context.Granted = true;
                }
            }
        }
        public void Complete(CheckAccessContext context) { }

        public void Adjust(CheckAccessContext context) {}

        private static bool IsCurrentUser(IUser currentUser, IContent user) {
            if (currentUser == null || user == null)
                return false;

            if (!user.Is<IUser>())
                return false;

            return currentUser.Id == user.Id;
        }
    }
}
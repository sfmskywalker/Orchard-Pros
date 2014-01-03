using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Security;
using OrchardPros.Models;

namespace OrchardPros.Security {
    [UsedImplicitly]
    public class RecommendationAuthorizationEventHandler : IAuthorizationServiceEventHandler {
        public void Checking(CheckAccessContext context) {
            if (!context.Granted) {
                if (context.Permission.Name == Permissions.WriteRecommendation.Name) {
                    context.Granted = !IsCurrentUser(context.User, context.Content);
                }
                else if(context.Permission.Name == Permissions.PublishRecommendation.Name) {
                    context.Granted = IsRecipientAndNotCreator(context.User, context.Content);
                }
                else if (context.Permission.Name == Permissions.DeleteRecommendation.Name) {
                    context.Granted = IsRecipient(context.User, context.Content);
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

        private bool IsRecipient(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var recommendationPart = content.As<RecommendationPart>();

            if (recommendationPart == null)
                return false;

            return recommendationPart.UserId == user.Id;
        }

        private bool IsRecipientAndNotCreator(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var recommendationPart = content.As<RecommendationPart>();

            if (recommendationPart == null)
                return false;

            return recommendationPart.RecommendingUser.Id != user.Id && recommendationPart.UserId == user.Id;
        }
    }
}
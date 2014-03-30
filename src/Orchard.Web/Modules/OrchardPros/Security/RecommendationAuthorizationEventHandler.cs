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
                    context.Granted = IsRecommendedUserAndNotCreator(context.User, context.Content);
                }
                else if (context.Permission.Name == Permissions.DeleteRecommendation.Name) {
                    context.Granted = IsRecommendedUser(context.User, context.Content);
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

        private bool IsRecommendedUser(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var recommendationPart = content.As<RecommendationPart>();

            if (recommendationPart == null)
                return false;

            return recommendationPart.RecommendedUserId == user.Id;
        }

        private bool IsRecommendedUserAndNotCreator(IUser user, IContent content) {
            if (user == null || content == null)
                return false;

            var recommendationPart = content.As<RecommendationPart>();

            if (recommendationPart == null)
                return false;

            return recommendationPart.RecommendingUser.Id != user.Id && recommendationPart.RecommendedUserId == user.Id;
        }
    }
}
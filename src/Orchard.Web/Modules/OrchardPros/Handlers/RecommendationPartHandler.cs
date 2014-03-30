using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Security;
using OrchardPros.Models;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class RecommendationPartHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        private readonly IUserManager _userManager;

        public RecommendationPartHandler(IRepository<RecommendationPartRecord> repository, IContentManager contentManager, IUserManager userManager) {
            _contentManager = contentManager;
            _userManager = userManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<RecommendationPart>(SetupLazyFields);
            OnCreated<RecommendationPart>(OnRecommendationCreated);
        }

        private void OnRecommendationCreated(CreateContentContext context, RecommendationPart part) {
            _userManager.AddActivityPoints(part.RecommendingUser, 2);
        }

        private void SetupLazyFields(ActivatedContentContext context, RecommendationPart part) {
            part.UserField.Loader(() => _contentManager.Get<IUser>(part.RecommendedUserId));
            part.UserField.Setter(x => {
                part.RecommendedUserId = x.Id;
                return x;
            });
        }
    }
}
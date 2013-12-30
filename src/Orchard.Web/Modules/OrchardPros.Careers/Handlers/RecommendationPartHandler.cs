using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Security;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Handlers {
    public class RecommendationPartHandler : ContentHandler {
        private readonly IContentManager _contentManager;

        public RecommendationPartHandler(IRepository<RecommendationPartRecord> repository, IContentManager contentManager) {
            _contentManager = contentManager;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<RecommendationPart>(SetupLazyFields);
        }

        private void SetupLazyFields(ActivatedContentContext context, RecommendationPart part) {
            part.UserField.Loader(() => _contentManager.Get<IUser>(part.UserId));
            part.UserField.Setter(x => {
                part.UserId = x.Id;
                return x;
            });
        }
    }
}
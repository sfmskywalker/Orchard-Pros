using System;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public class RecommendationManager : IRecommendationManager {
        private readonly IContentManager _contentManager;

        public RecommendationManager(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public RecommendationPart Create(Action<RecommendationPart> initialize = null) {
            return _contentManager.Create("Recommendation", VersionOptions.Draft, initialize);
        }

        public IContentQuery<ContentItem, RecommendationPartRecord> GetByUser(int userId) {
            return _contentManager.Query(VersionOptions.Latest).Join<RecommendationPartRecord>().Where(x => x.UserId == userId);
        }
    }
}
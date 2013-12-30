using System;
using Orchard.ContentManagement;
using Orchard.Services;
using OrchardPros.Careers.Models;

namespace OrchardPros.Careers.Services {
    public class RecommendationManager : IRecommendationManager {
        private readonly IClock _clock;
        private readonly IContentManager _contentManager;

        public RecommendationManager(IClock clock, IContentManager contentManager) {
            _clock = clock;
            _contentManager = contentManager;
        }

        public RecommendationPart Create(Action<RecommendationPart> initialize = null) {
            return _contentManager.Create("Recommendation", initialize);
        }

        public IContentQuery<ContentItem, RecommendationPartRecord> GetByUser(int userId) {
            return _contentManager.Query().Join<RecommendationPartRecord>().Where(x => x.UserId == userId);
        }

        public void Approve(RecommendationPart recommendation) {
            recommendation.Approved = true;
            recommendation.ApprovedUtc = _clock.UtcNow;
        }
    }
}
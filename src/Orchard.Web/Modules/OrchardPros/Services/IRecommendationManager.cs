using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IRecommendationManager : IDependency {
        RecommendationPart Create(Action<RecommendationPart> initialize = null);
        IContentQuery<ContentItem, RecommendationPartRecord> GetByUser(int userId);
        void Approve(RecommendationPart recommendation);
    }
}
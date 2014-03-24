using System;
using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public interface IRecommendationManager : IDependency {
        RecommendationPart Create(Action<RecommendationPart> initialize = null);
        IContentQuery<ContentItem, RecommendationPartRecord> GetByUser(int userId);
    }
}
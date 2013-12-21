using System.Collections.Generic;
using System.Linq;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Security;
using OrchardPros.Careers.Services;
using OrchardPros.Careers.ViewModels;

namespace OrchardPros.Careers {
    public class Shapes : IShapeTableProvider {
        private readonly Work<IRecommendationManager> _recommendationManager;

        public Shapes(Work<IRecommendationManager> recommendationManager) {
            _recommendationManager = recommendationManager;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Profile").OnDisplaying(context => {
                var user = (IUser) context.Shape.User;
                context.Shape.Recommendations = GetRecommendationsFor(user).ToList();
            });
        }

        private IEnumerable<RecommendationEx> GetRecommendationsFor(IUser user) {
            return _recommendationManager.Value.FetchEx(user.Id);
        }
    }
}
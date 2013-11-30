using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Handlers {
    public class ProfessionalProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;
        private readonly IRecommendationManager _recommendationManager;

        public ProfessionalProfilePartHandler(IPositionManager positionManager, ISkillManager skillManager, IRecommendationManager recommendationManager) {
            _positionManager = positionManager;
            _skillManager = skillManager;
            _recommendationManager = recommendationManager;
            OnActivated<ProfessionalProfilePart>(SetupFields);
        }

        private void SetupFields(ActivatedContentContext context, ProfessionalProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToList());
            part.SkillsField.Loader(() => _skillManager.Fetch(part.Id).ToList());
            part.RecommendationsField.Loader(() => _recommendationManager.Fetch(part.Id).ToList());
        }
    }
}
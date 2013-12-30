﻿using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Handlers {
    public class ProfessionalProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly IExperienceManager _experienceManager;

        public ProfessionalProfilePartHandler(
            IPositionManager positionManager, 
            ISkillManager skillManager, 
            IRecommendationManager recommendationManager, 
            IExperienceManager experienceManager) {

            _positionManager = positionManager;
            _skillManager = skillManager;
            _recommendationManager = recommendationManager;
            _experienceManager = experienceManager;
            OnActivated<ProfessionalProfilePart>(SetupFields);
        }

        private void SetupFields(ActivatedContentContext context, ProfessionalProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToArray());
            part.SkillsField.Loader(() => _skillManager.Fetch(part.Id).ToArray());
            part.RecommendationsField.Loader(() => _recommendationManager.GetByUser(part.Id).List<RecommendationPart>().ToArray());
            part.ExperienceField.Loader(() => _experienceManager.Fetch(part.Id).ToArray());
        }
    }
}
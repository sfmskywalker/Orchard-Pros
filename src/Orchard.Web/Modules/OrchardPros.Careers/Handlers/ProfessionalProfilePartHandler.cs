using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Handlers {
    public class ProfessionalProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;

        public ProfessionalProfilePartHandler(IPositionManager positionManager, ISkillManager skillManager) {
            _positionManager = positionManager;
            _skillManager = skillManager;
            OnActivated<ProfessionalProfilePart>(SetupFields);
        }

        private void SetupFields(ActivatedContentContext context, ProfessionalProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToList());
            part.SkillsField.Loader(() => _skillManager.Fetch(part.Id).ToList());
        }
    }
}
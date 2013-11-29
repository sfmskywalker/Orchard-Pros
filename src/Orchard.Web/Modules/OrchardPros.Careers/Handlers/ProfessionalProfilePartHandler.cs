using System.Linq;
using Orchard.ContentManagement.Handlers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Handlers {
    public class ProfessionalProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;

        public ProfessionalProfilePartHandler(IPositionManager positionManager) {
            _positionManager = positionManager;
            OnActivated<ProfessionalProfilePart>(SetupFields);
        }

        private void SetupFields(ActivatedContentContext context, ProfessionalProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToList());
        }
    }
}
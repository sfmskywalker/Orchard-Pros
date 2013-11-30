using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Drivers {
    public class ProfessionalProfilePartDriver : ContentPartDriver<ProfessionalProfilePart> {
        private readonly IRecommendationManager _recommendationManager;
        public ProfessionalProfilePartDriver(IRecommendationManager recommendationManager) {
            _recommendationManager = recommendationManager;
        }

        protected override DriverResult Display(ProfessionalProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile", () => shapeHelper.Parts_ProfessionalProfile);
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile_Edit", () => {
                var viewModel = shapeHelper.ViewModel(
                    Model: part,
                    Positions: shapeHelper.ProfessionalProfile_Edit_Positions(Profile: part, Positions: part.Positions.ToList()),
                    Skills: shapeHelper.ProfessionalProfile_Edit_Skills(Profile: part, Skills: part.Skills.ToList()),
                    Recommendations: shapeHelper.ProfessionalProfile_Edit_Recommendations(Profile: part, Recommendations: _recommendationManager.FetchEx(part.Id).ToList()));

                return shapeHelper.EditorTemplate(TemplateName: "Parts/ProfessionalProfile", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
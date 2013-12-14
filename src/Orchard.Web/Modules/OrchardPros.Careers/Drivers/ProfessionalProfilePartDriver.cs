using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Drivers {
    public class ProfessionalProfilePartDriver : ContentPartDriver<ProfessionalProfilePart> {
        private readonly IRecommendationManager _recommendationManager;
        private readonly IExperienceManager _experienceManager;

        public ProfessionalProfilePartDriver(IRecommendationManager recommendationManager, IExperienceManager experienceManager) {
            _recommendationManager = recommendationManager;
            _experienceManager = experienceManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(ProfessionalProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile", () => shapeHelper.Parts_ProfessionalProfile);
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile_Edit", () => {
                var viewModel = shapeHelper.ViewModel(Model: part);
                viewModel.Add(shapeHelper.ProfessionalProfile_Edit_Positions(TabText: T("Positions"), Profile: part, Positions: part.Positions.ToList()));
                viewModel.Add(shapeHelper.ProfessionalProfile_Edit_Skills(TabText: T("Skills"), Profile: part, Skills: part.Skills.ToList()));
                viewModel.Add(shapeHelper.ProfessionalProfile_Edit_Recommendations(TabText: T("Recommendations"), Profile: part, Recommendations: _recommendationManager.FetchEx(part.Id).ToList()));
                viewModel.Add(shapeHelper.ProfessionalProfile_Edit_Experience(TabText: T("Experience"), Profile: part, Experience: _experienceManager.Fetch(part.Id).ToList()));
                return shapeHelper.EditorTemplate(TemplateName: "Parts/ProfessionalProfile", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
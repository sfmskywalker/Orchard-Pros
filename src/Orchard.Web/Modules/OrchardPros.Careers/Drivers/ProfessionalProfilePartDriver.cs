using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using OrchardPros.Careers.Helpers;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;
using OrchardPros.Careers.ViewModels;

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
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(ProfessionalProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_ProfessionalProfile_Edit", () => {
                var viewModel = new ProfessionalProfileViewModel {
                    Title = part.Title,
                    Location = part.Location,
                    Tabs = shapeHelper.Tabs()
                };
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Positions(TabText: T("Positions"), Profile: part, Positions: part.Positions.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Skills(TabText: T("Skills"), Profile: part, Skills: part.Skills.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Recommendations(TabText: T("Recommendations"), Profile: part, Recommendations: _recommendationManager.FetchEx(part.Id).ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Experience(TabText: T("Experience"), Profile: part, Experience: _experienceManager.Fetch(part.Id).ToList()));

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] {"Tabs"})) {
                        part.Title = viewModel.Title.TrimSafe();
                        part.Location = viewModel.Location.TrimSafe();
                    }
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/ProfessionalProfile", Model: viewModel, Prefix: Prefix);
            });
        }
    }
}
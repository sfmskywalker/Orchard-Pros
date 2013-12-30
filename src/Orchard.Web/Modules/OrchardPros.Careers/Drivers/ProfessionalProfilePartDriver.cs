using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;
using OrchardPros.Careers.ViewModels;

namespace OrchardPros.Careers.Drivers {
    public class ProfessionalProfilePartDriver : ContentPartDriver<ProfessionalProfilePart> {
        private readonly IExperienceManager _experienceManager;
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;

        public ProfessionalProfilePartDriver(
            IExperienceManager experienceManager, 
            IPositionManager positionManager, 
            ISkillManager skillManager) {
            _experienceManager = experienceManager;
            _positionManager = positionManager;
            _skillManager = skillManager;
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
                    Tabs = shapeHelper.Tabs()
                };
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Positions(TabText: T("Positions"), Profile: part, Positions: part.Positions.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Skills(TabText: T("Skills"), Profile: part, Skills: part.Skills.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Recommendations(TabText: T("Recommendations"), Profile: part));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Experience(TabText: T("Experience"), Profile: part, Experience: _experienceManager.Fetch(part.Id).ToList()));

                return shapeHelper.EditorTemplate(TemplateName: "Parts/ProfessionalProfile", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override void Exporting(ProfessionalProfilePart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            partElement.Add(CreatePositionsElement(part));
            partElement.Add(CreateSkillsElement(part));
            partElement.Add(CreateExperiencesElement(part));
        }

        protected override void Importing(ProfessionalProfilePart part, ImportContentContext context) {
            var partElement = context.Data.Element(part.PartDefinition.Name);
            var positionsDictionary = ImportPositions(part, partElement);
            ImportSkills(part, partElement);
            ImportExperiences(part, partElement, positionsDictionary);
        }

        private void ImportExperiences(ProfessionalProfilePart part, XElement partElement, IDictionary<int, Position> positionsDictionary) {
            var experiencesElement = partElement.Element("Experiences");

            if (experiencesElement != null) {
                foreach (var experienceElement in experiencesElement.Elements("Experience")) {
                    var element = experienceElement;
                    _experienceManager.Create(part.Id, e => {
                        e.CreatedUtc = element.Attr<DateTime>("CreatedUtc");
                        e.Description = element.Value;
                        e.Position = positionsDictionary[element.Attr<int>("LocalPositionId")];
                    });
                }
            }
        }

        private void ImportSkills(ProfessionalProfilePart part, XElement partElement) {
            var skillsElement = partElement.Element("Skills");

            if (skillsElement != null) {
                foreach (var skillElement in skillsElement.Elements("Skill")) {
                    var element = skillElement;
                    _skillManager.Create(part.Id, s => {
                        s.Name = element.Attr<string>("Name");
                        s.Rating = element.Attr<int>("Rating");
                    });
                }
            }
        }

        private IDictionary<int, Position> ImportPositions(ProfessionalProfilePart part, XElement partElement) {
            var positionsElement = partElement.Element("Positions");
            var dictionary = new Dictionary<int, Position>();

            if (positionsElement != null) {
                foreach (var positionElement in positionsElement.Elements("Position")) {
                    var localId = positionElement.Attr<int>("LocalId");
                    var element = positionElement;
                    var position = _positionManager.Create(part.Id, p => {
                        p.ArchivedUtc = element.Attr<DateTime?>("ArchivedUtc");
                        p.CompanyName = element.Attr<string>("CompanyName");
                        p.Description = element.Val<string>();
                        p.IsArchived = element.Attr<bool>("IsArchived");
                        p.IsCurrentPosition = element.Attr<bool>("IsCurrentPosition");
                        p.Location = element.Attr<string>("Location");
                        p.PeriodEndMonth = element.Attr<int?>("PeriodEndMonth");
                        p.PeriodEndYear = element.Attr<int?>("PeriodEndYear");
                        p.PeriodStartYear = element.Attr<int?>("PeriodStartYear");
                        p.PeriodStartMonth = element.Attr<int?>("PeriodStartMonth");
                        p.Title = element.Attr<string>("Title");
                    });
                    dictionary.Add(localId, position);
                }
            }
            return dictionary;
        }

        private XElement CreateExperiencesElement(ProfessionalProfilePart part) {
            return new XElement("Experiences", 
                part.Experience.Select(experience => 
                    new XElement("Experience",
                        new XAttribute("LocalPositionId", experience.Position.Id),
                        new XAttribute("CreatedUtc", experience.CreatedUtc),
                        new XText(experience.Description))));
        }

        private XElement CreateSkillsElement(ProfessionalProfilePart part) {
            return new XElement("Skills", 
                part.Skills.Select(skill => 
                    new XElement("Skill", 
                        new XAttribute("Name", skill.Name), 
                        new XAttribute("Rating", skill.Rating))));
        }

        private static XElement CreatePositionsElement(ProfessionalProfilePart part) {
            var positionsElement = new XElement("Positions");

            foreach (var position in part.Positions) {
                var positionElement = new XElement("Position");

                positionsElement.Add(positionElement);
                positionElement.Add(new XAttribute("LocalId", position.Id));
                positionElement.Add(new XAttribute("CompanyName", position.CompanyName));
                positionElement.Add(new XAttribute("Title", position.Title));
                positionElement.Add(new XAttribute("Location", position.Location));
                positionElement.Add(new XAttribute("IsCurrentPosition", position.IsCurrentPosition));
                positionElement.Add(new XAttribute("IsArchived", position.IsArchived));

                if(position.ArchivedUtc != null)
                    positionElement.Add(new XAttribute("ArchivedUtc", position.ArchivedUtc));

                if (position.PeriodStartYear != null)
                    positionElement.Add(new XAttribute("PeriodStartYear", position.PeriodStartYear));

                if (position.PeriodStartMonth != null)
                    positionElement.Add(new XAttribute("PeriodStartMonth", position.PeriodStartMonth));

                if (position.PeriodEndYear != null)
                    positionElement.Add(new XAttribute("PeriodEndYear", position.PeriodEndYear));

                if (position.PeriodEndMonth != null)
                    positionElement.Add(new XAttribute("PeriodEndMonth", position.PeriodEndMonth));

                if(!String.IsNullOrWhiteSpace(position.Description))
                    positionElement.Value = position.Description;
            }

            return positionsElement;
        }
    }
}
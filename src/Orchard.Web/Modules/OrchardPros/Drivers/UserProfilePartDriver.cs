﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services.User;
using OrchardPros.ViewModels;

namespace OrchardPros.Drivers {
    public class UserProfilePartDriver : ContentPartDriver<UserProfilePart> {
        private readonly IExperienceManager _experienceManager;
        private readonly ISkillManager _skillManager;
        private readonly IPositionManager _positionManager;

        public UserProfilePartDriver(IExperienceManager experienceManager, ISkillManager skillManager, IPositionManager positionManager) {
            _experienceManager = experienceManager;
            _skillManager = skillManager;
            _positionManager = positionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Display(UserProfilePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile", () => shapeHelper.Parts_UserProfile());
        }

        protected override DriverResult Editor(UserProfilePart part, dynamic shapeHelper) {
            return Editor(part, null, shapeHelper);
        }

        protected override DriverResult Editor(UserProfilePart part, IUpdateModel updater, dynamic shapeHelper) {
            return ContentShape("Parts_UserProfile_Edit", () => {
                var viewModel = new UserProfileViewModel {
                    FirstName = part.FirstName,
                    MiddleName = part.MiddleName,
                    LastName = part.LastName,
                    AvatarType = part.AvatarType,
                    Bio = part.Bio,
                    Level = part.Level,
                    ExperiencePoints = part.ExperiencePoints,
                    TwitterAlias = part.TwitterAlias,
                    FacebookUrl = part.FacebookUrl,
                    BlogUrl = part.BlogUrl,
                    CompanyWebsiteUrl = part.CompanyWebsiteUrl,
                    LinkedInUrl = part.LinkedInUrl,
                    Tabs = shapeHelper.Tabs()
                };
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Positions(TabText: T("Positions"), Profile: part, Positions: part.Positions.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Skills(TabText: T("Skills"), Profile: part, Skills: part.Skills.ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Recommendations(TabText: T("Recommendations"), Profile: part));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Experience(TabText: T("Experience"), Profile: part, Experience: _experienceManager.Fetch(part.Id).ToList()));
                viewModel.Tabs.Add(shapeHelper.ProfessionalProfile_Edit_Social(TabText: T("Social"), Profile: part));

                if (updater != null) {
                    if (updater.TryUpdateModel(viewModel, Prefix, null, new[] {"Tabs"})) {
                        part.FirstName = viewModel.FirstName.TrimSafe();
                        part.MiddleName = viewModel.MiddleName.TrimSafe();
                        part.LastName = viewModel.LastName.TrimSafe();
                        part.AvatarType = viewModel.AvatarType;
                        part.Bio = viewModel.Bio;
                        part.Level = viewModel.Level;
                        part.ExperiencePoints = viewModel.ExperiencePoints;
                        part.TwitterAlias = viewModel.TwitterAlias.TrimSafe();
                        part.FacebookUrl = viewModel.FacebookUrl.TrimSafe();
                        part.LinkedInUrl = viewModel.LinkedInUrl.TrimSafe();
                        part.CompanyWebsiteUrl = viewModel.CompanyWebsiteUrl.TrimSafe();
                        part.BlogUrl = viewModel.BlogUrl.TrimSafe();
                    }
                }

                return shapeHelper.EditorTemplate(TemplateName: "Parts/UserProfile", Model: viewModel, Prefix: Prefix);
            });
        }

        protected override void Exporting(UserProfilePart part, ExportContentContext context) {
            var partElement = context.Element(part.PartDefinition.Name);
            partElement.SetAttributeValue("FirstName", part.FirstName);
            partElement.SetAttributeValue("MiddleName", part.MiddleName);
            partElement.SetAttributeValue("LastName", part.LastName);
            partElement.SetAttributeValue("AvatarType", part.AvatarType);
            partElement.SetAttributeValue("CreatedUtc", part.CreatedUtc);
            partElement.SetAttributeValue("LastLoggedInUtc", part.LastLoggedInUtc);
            partElement.SetAttributeValue("Level", part.Level);
            partElement.SetAttributeValue("ExperiencePoints", part.ExperiencePoints);
            partElement.SetAttributeValue("TwitterAlias", part.TwitterAlias);
            partElement.SetAttributeValue("FacebookUrl", part.FacebookUrl);
            partElement.SetAttributeValue("LinkedInUrl", part.LinkedInUrl);
            partElement.SetAttributeValue("CompanyWebsiteUrl", part.CompanyWebsiteUrl);
            partElement.SetAttributeValue("BlogUrl", part.BlogUrl);
            partElement.Add(CreatePositionsElement(part));
            partElement.Add(CreateSkillsElement(part));
            partElement.Add(CreateExperiencesElement(part));
            partElement.Add(CreateBioElement(part));
        }

        protected override void Importing(UserProfilePart part, ImportContentContext context) {
            var partElement = context.Data.Element(part.PartDefinition.Name);
            var bioElement = partElement.Element("Bio");
            var positionsDictionary = ImportPositions(part, partElement);

            context.ImportAttribute(part.PartDefinition.Name, "FirstName", x => part.FirstName = x);
            context.ImportAttribute(part.PartDefinition.Name, "MiddleName", x => part.MiddleName = x);
            context.ImportAttribute(part.PartDefinition.Name, "LastName", x => part.LastName = x);
            context.ImportAttribute(part.PartDefinition.Name, "AvatarType", x => part.AvatarType = (AvatarType)Enum.Parse(typeof(AvatarType), x));
            context.ImportAttribute(part.PartDefinition.Name, "CreatedUtc", x => part.CreatedUtc = XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.Utc));
            context.ImportAttribute(part.PartDefinition.Name, "LastLoggedInUtc", x => part.LastLoggedInUtc = XmlConvert.ToDateTime(x, XmlDateTimeSerializationMode.Utc));
            context.ImportAttribute(part.PartDefinition.Name, "Level", x => part.Level = XmlConvert.ToInt32(x));
            context.ImportAttribute(part.PartDefinition.Name, "ExperiencePoints", x => part.ExperiencePoints = XmlConvert.ToInt32(x));
            context.ImportAttribute(part.PartDefinition.Name, "TwitterAlias", x => part.TwitterAlias = x);
            context.ImportAttribute(part.PartDefinition.Name, "FacebookUrl", x => part.FacebookUrl = x);
            context.ImportAttribute(part.PartDefinition.Name, "LinkedInUrl", x => part.LinkedInUrl = x);
            context.ImportAttribute(part.PartDefinition.Name, "CompanyWebsiteUrl", x => part.CompanyWebsiteUrl = x);
            context.ImportAttribute(part.PartDefinition.Name, "BlogUrl", x => part.BlogUrl = x);

            part.Bio = bioElement != null ? bioElement.Value : null;
            ImportSkills(part, partElement);
            ImportExperiences(part, partElement, positionsDictionary);
        }

        private void ImportExperiences(UserProfilePart part, XElement partElement, IDictionary<int, Position> positionsDictionary) {
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

        private void ImportSkills(UserProfilePart part, XElement partElement) {
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

        private IDictionary<int, Position> ImportPositions(UserProfilePart part, XElement partElement) {
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

        private XElement CreateExperiencesElement(UserProfilePart part) {
            return new XElement("Experiences",
                part.Experience.Select(experience =>
                    new XElement("Experience",
                        new XAttribute("LocalPositionId", experience.Position.Id),
                        new XAttribute("CreatedUtc", experience.CreatedUtc),
                        new XText(experience.Description))));
        }

        private XElement CreateBioElement(UserProfilePart part) {
            return new XElement("Bio", part.Bio);
        }

        private XElement CreateSkillsElement(UserProfilePart part) {
            return new XElement("Skills",
                part.Skills.Select(skill =>
                    new XElement("Skill",
                        new XAttribute("Name", skill.Name),
                        new XAttribute("Rating", skill.Rating))));
        }

        private static XElement CreatePositionsElement(UserProfilePart part) {
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

                if (position.ArchivedUtc != null)
                    positionElement.Add(new XAttribute("ArchivedUtc", position.ArchivedUtc));

                if (position.PeriodStartYear != null)
                    positionElement.Add(new XAttribute("PeriodStartYear", position.PeriodStartYear));

                if (position.PeriodStartMonth != null)
                    positionElement.Add(new XAttribute("PeriodStartMonth", position.PeriodStartMonth));

                if (position.PeriodEndYear != null)
                    positionElement.Add(new XAttribute("PeriodEndYear", position.PeriodEndYear));

                if (position.PeriodEndMonth != null)
                    positionElement.Add(new XAttribute("PeriodEndMonth", position.PeriodEndMonth));

                if (!String.IsNullOrWhiteSpace(position.Description))
                    positionElement.Value = position.Description;
            }

            return positionsElement;
        }
    }
}
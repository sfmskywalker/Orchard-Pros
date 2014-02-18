using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.Core.Common.Utilities;
using Orchard.MediaLibrary.Fields;
using Orchard.Security;
using OrchardPros.Helpers;

namespace OrchardPros.Models {
    public class UserProfilePart : ContentPart<UserProfilePartRecord> {
        internal LazyField<IEnumerable<Position>> PositionsField = new LazyField<IEnumerable<Position>>();
        internal LazyField<IEnumerable<Skill>> SkillsField = new LazyField<IEnumerable<Skill>>();
        internal LazyField<IEnumerable<RecommendationPart>> RecommendationsField = new LazyField<IEnumerable<RecommendationPart>>();
        internal LazyField<IEnumerable<Experience>> ExperienceField = new LazyField<IEnumerable<Experience>>();
        internal LazyField<IEnumerable<ReplyPart>> RepliesField = new LazyField<IEnumerable<ReplyPart>>();
        internal LazyField<IEnumerable<TicketPart>> SolvedTicketsField = new LazyField<IEnumerable<TicketPart>>();
        internal LazyField<int> RatingField = new LazyField<int>();

        public string FirstName {
            get { return this.Retrieve(x => x.FirstName); }
            set { this.Store(x => x.FirstName, value); }
        }

        public string LastName {
            get { return this.Retrieve(x => x.LastName); }
            set { this.Store(x => x.LastName, value); }
        }

        public string MiddleName {
            get { return this.Retrieve(x => x.MiddleName); }
            set { this.Store(x => x.MiddleName, value); }
        }

        public Country Country {
            get { return Record.Country; }
            set { Record.Country = value; }
        }

        public string Bio {
            get { return this.Retrieve(x => x.Bio); }
            set { this.Store(x => x.Bio, value); }
        }

        public string Avatar {
            get { 
                return this.FieldValue<MediaLibraryPickerField, string>("Avatar", x => {
                    var media = x.MediaParts.FirstOrDefault();
                    return media != null ? media.MediaUrl : null;
                });
            }
        }

        public MediaLibraryPickerField AvatarField {
            get { return this.Field<MediaLibraryPickerField>("Avatar"); }
        }

        public MediaLibraryPickerField WallpaperField {
            get { return this.Field<MediaLibraryPickerField>("Wallpaper"); }
        }

        public AvatarType AvatarType {
            get { return this.Retrieve(x => x.AvatarType); }
            set { this.Store(x => x.AvatarType, value); }
        }

        public IEnumerable<NotificationSetting> NotificationSettings {
            get {
                var infoSet = this.As<InfosetPart>().Infoset;
                var notificationSettingsElement = infoSet.Element.Element("NotificationSettings") ?? new XElement("NotificationSettings");
                var notificationElements = notificationSettingsElement.Elements("Notification");
                return notificationElements.Select(x => new NotificationSetting {Name = x.Attr<string>("Name")});
            }
            set {
                var infoSet = this.As<InfosetPart>().Infoset;
                var notificationSettingsElement = infoSet.Element.Element("NotificationSettings");

                if (notificationSettingsElement != null) {
                    notificationSettingsElement.Remove();
                }

                if (value == null)
                    return;

                notificationSettingsElement = new XElement("NotificationSettings", 
                    value.Select(x => new XElement("Notification", new XAttribute("Name", x.Name))));

                infoSet.Element.Add(notificationSettingsElement);
            }
        }

        public IDictionary<string, UserPayoutProvider> PayoutProviders {
            get {
                var infoSet = this.As<InfosetPart>().Infoset;
                var providersElement = infoSet.Element.Element("PayoutProviders") ?? new XElement("PayoutProviders");
                var providerElements = providersElement.Elements("Provider");
                return providerElements.Select(x => new UserPayoutProvider {
                    ProviderName = x.Attr("ProviderName"), 
                    AccessToken = x.Attr("AccessToken"),
                    RefreshToken = x.Attr("RefreshToken")
                }).ToDictionary(x => x.ProviderName);
            }
            set {
                var infoSet = this.As<InfosetPart>().Infoset;
                var providersElement = infoSet.Element.Element("PayoutProviders");

                if(providersElement != null)
                    providersElement.Remove();

                if (value == null)
                    return;

                providersElement = new XElement("PayoutProviders", 
                    value.Values.Select(x => new XElement("Provider", 
                        new XAttribute("ProviderName", x.ProviderName),
                        new XAttribute("AccessToken", x.AccessToken),
                        new XAttribute("RefreshToken", x.RefreshToken))));

                infoSet.Element.Add(providersElement);
            }
        }

        public DateTime CreatedUtc {
            get { return this.Retrieve(x => x.CreatedUtc); }
            set { this.Store(x => x.CreatedUtc, value); }
        }

        public DateTime? LastLoggedInUtc {
            get { return this.Retrieve(x => x.LastLoggedInUtc); }
            set { this.Store(x => x.LastLoggedInUtc, value); }
        }

        public string FullName {
            get { return String.Format("{0} {1}", FirstName, String.IsNullOrWhiteSpace(MiddleName) ? LastName : MiddleName + " " + LastName); }
        }

        public string FullLastName {
            get { return String.IsNullOrWhiteSpace(MiddleName) ? LastName : MiddleName + " " + LastName; }
        }

        public string DisplayName {
            get {
                var fullName = FullName;
                return String.IsNullOrWhiteSpace(fullName) ? this.As<IUser>().UserName : fullName;
            }
        }

        public int Level {
            get { return this.Retrieve(x => x.Level); }
            set { this.Store(x => x.Level, value); }
        }

        public int ExperiencePoints {
            get { return Retrieve(x => x.ExperiencePoints); }
            set { Store(x => x.ExperiencePoints, value); }
        }

        public int Rating {
            get { return RatingField.Value; }
        }

        public string RankName {
            get { return "Rookie"; }
        }

        public IEnumerable<Position> Positions {
            get { return PositionsField.Value; }
        }

        public IEnumerable<Skill> Skills {
            get { return SkillsField.Value; }
        }

        public IEnumerable<RecommendationPart> Recommendations {
            get { return RecommendationsField.Value; }
        }

        public IEnumerable<Experience> Experience {
            get { return ExperienceField.Value; }
        }

        public IEnumerable<ReplyPart> Replies {
            get { return RepliesField.Value; }
        }

        public IEnumerable<TicketPart> SolvedTickets {
            get { return SolvedTicketsField.Value; }
        }
    }
}
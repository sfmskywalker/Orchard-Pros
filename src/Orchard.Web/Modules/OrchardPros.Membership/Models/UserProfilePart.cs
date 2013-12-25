using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.MediaLibrary.Fields;
using Orchard.Security;
using OrchardPros.Membership.Helpers;

namespace OrchardPros.Membership.Models {
    public class UserProfilePart : ContentPart {
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

        public string Bio {
            get { return this.Retrieve(x => x.Bio); }
            set { this.Store(x => x.Bio, value); }
        }

        public string Avatar {
            get { return this.FieldValue<MediaLibraryPickerField, string>("Avatar", x => {
                var media = x.MediaParts.FirstOrDefault();
                return media != null ? media.MediaUrl : null;
            });}
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

                notificationSettingsElement = new XElement("NotificationSettings", value.Select(x => new XElement("Notification", new XAttribute("Name", x.Name))));
                infoSet.Element.Add(notificationSettingsElement);
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

        public string DisplayName {
            get {
                var fullName = FullName;
                return String.IsNullOrWhiteSpace(fullName) ? this.As<IUser>().UserName : fullName;
            }
        }
    }
}
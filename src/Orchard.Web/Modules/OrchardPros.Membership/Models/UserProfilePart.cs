using System;
using System.Linq;
using Orchard.ContentManagement;
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
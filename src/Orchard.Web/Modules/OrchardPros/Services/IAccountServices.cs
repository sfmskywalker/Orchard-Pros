using System.Web;
using Orchard;
using Orchard.Security;

namespace OrchardPros.Services {
    public interface IAccountServices : IDependency {
        void ChangeUserName(IUser user, string newUserName);
        void ChangeUserEmail(IUser user, string newEmail);
        void UpdateAvatar(IUser user, HttpPostedFileBase file);
        void DeleteAvatar(IUser user);
        void UpdateWallpaper(IUser user, HttpPostedFileBase file);
        void DeleteWallpaper(IUser user);
    }
}
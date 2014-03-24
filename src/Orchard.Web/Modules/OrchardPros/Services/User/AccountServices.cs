using System;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.FileSystems.Media;
using Orchard.MediaLibrary.Fields;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Orchard.Users.Models;
using OrchardPros.Models;

namespace OrchardPros.Services.User {
    public class AccountServices : IAccountServices {
        private readonly IRepository<UserPartRecord> _userRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IContentManager _contentManager;

        public AccountServices(IRepository<UserPartRecord> userRepository, IStorageProvider storageProvider, IMediaLibraryService mediaLibraryService, IContentManager contentManager) {
            _userRepository = userRepository;
            _storageProvider = storageProvider;
            _mediaLibraryService = mediaLibraryService;
            _contentManager = contentManager;
        }

        public void ChangeUserName(IUser user, string newUserName) {
            var record = _userRepository.Get(user.Id);
            user.As<UserPart>().UserName = newUserName;
            record.NormalizedUserName = newUserName.ToLowerInvariant();
        }

        public void ChangeUserEmail(IUser user, string newEmail) {
            user.As<UserPart>().Email = newEmail;
        }

        public void UpdateAvatar(IUser user, HttpPostedFileBase file) {
            UpdateMedia(user, user.As<UserProfilePart>().AvatarField, file);
        }

        public void UpdateWallpaper(IUser user, HttpPostedFileBase file) {
            UpdateMedia(user, user.As<UserProfilePart>().WallpaperField, file);
        }

        public void DeleteAvatar(IUser user) {
            DeleteMedia(user.As<UserProfilePart>().AvatarField);
        }

        public void DeleteWallpaper(IUser user) {
            DeleteMedia(user.As<UserProfilePart>().WallpaperField);
        }

        private void UpdateMedia(IUser user, MediaLibraryPickerField field, HttpPostedFileBase file) {
            DeleteMedia(field);

            var folderPath = String.Format("Users\\{0:000000}\\Pictures", user.Id);
            var fileName = Path.GetFileName(file.FileName);
            var mediaPart = _mediaLibraryService.ImportMedia(file.InputStream, folderPath, fileName);

            _contentManager.Create(mediaPart, VersionOptions.Published);
            field.Ids = new[] { mediaPart.Id };
        }

        private void DeleteMedia(MediaLibraryPickerField field) {
            var existingMedia = field.MediaParts.FirstOrDefault();

            if (existingMedia != null) {
                _contentManager.Remove(existingMedia.ContentItem);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.FileSystems.Media;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class AttachmentService : IAttachmentService {
        private const string AttachmentsFolderPath = "_Attachments";
        private readonly IContentManager _contentManager;
        private readonly IStorageProvider _storageProvider;

        public AttachmentService(IContentManager contentManager, IStorageProvider storageProvider) {
            _contentManager = contentManager;
            _storageProvider = storageProvider;
        }

        public string UploadAttachment(HttpPostedFileBase file) {
            var extension = Path.GetExtension(file.FileName);
            var fileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
            var path = AttachmentsFolderPath + "/" + fileName;
            _storageProvider.SaveStream(path, file.InputStream);
            return fileName;
        }

        public void AssociateAttachments(IContent content, IEnumerable<string> uploadedFileNames, IList<string> uploadedFileContentTypes, IEnumerable<string> originalFileNames) {
            if (uploadedFileNames == null || originalFileNames == null)
                return;

            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();
            var currentAttachments = content.As<AttachmentsHolderPart>().Attachments.ToList();

            // Delete current attachments (unless they are part of the originalFileNames list).
            foreach (var attachment in currentAttachments.Where(x => !originalFiles.Contains(x.OriginalFileName, StringComparer.OrdinalIgnoreCase)).ToArray()) {
                _contentManager.Remove(attachment.ContentItem);
                currentAttachments.Remove(attachment);
            }

            _storageProvider.TryCreateFolder(AttachmentsFolderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                if (String.IsNullOrEmpty(uploadedFiles[i]))
                    continue; // This file was already uploaded, we're just receiving back the filename.

                var generatedFileName = uploadedFiles[i];
                var mimeType = uploadedFileContentTypes[i];
                var uploadedFilePath = AttachmentsFolderPath + "/" + generatedFileName;
                var originalFileName = originalFiles[i];
                var attachment = _contentManager.Create<AttachmentPart>("Attachment", a => {
                    a.As<CommonPart>().Container = content;
                    a.OriginalFileName = originalFileName;
                    a.LocalFileName = generatedFileName;
                    a.FileSize = _storageProvider.GetFile(uploadedFilePath).GetSize();
                    a.MimeType = mimeType;
                });

                currentAttachments.Add(attachment);
            }

            content.As<AttachmentsHolderPart>().Attachments = currentAttachments;
        }

        public void DeleteAttachments(IContent content) {
            var currentAttachments = content.As<AttachmentsHolderPart>().Attachments.ToList();
            foreach (var attachment in currentAttachments) {
                _contentManager.Remove(attachment.ContentItem);
            }
        }

        public void DeleteFileFor(AttachmentPart attachment) {
            var filePath = GetFilePath(attachment);

            if (_storageProvider.FileExists(filePath)) {
                _storageProvider.DeleteFile(filePath);
            }
        }

        public AttachmentPart GetAttachment(int id) {
            return _contentManager.Get<AttachmentPart>(id);
        }

        public AttachmentPart GetAttachmentByIdentifier(string identifier) {
            return _contentManager
                .Query<IdentityPart, IdentityPartRecord>()
                .Where(x => x.Identifier == identifier)
                .ForPart<AttachmentPart>().List()
                .FirstOrDefault();
        }

        public IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids) {
            return _contentManager.GetMany<AttachmentPart>(ids, VersionOptions.Published, QueryHints.Empty);
        }

        public string GetFilePath(AttachmentPart attachment) {
            return AttachmentsFolderPath + "/" + attachment.LocalFileName;
        }

        public Stream OpenRead(AttachmentPart attachment) {
            return _storageProvider.GetFile(GetFilePath(attachment)).OpenRead();
        }
    }
}
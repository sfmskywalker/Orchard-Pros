using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Common.Models;
using Orchard.FileSystems.Media;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class AttachmentService : IAttachmentService {
        private const string TempFolderPath = "_Attachments/_Temp";
        private readonly IContentManager _contentManager;
        private readonly IStorageProvider _storageProvider;

        public AttachmentService(IContentManager contentManager, IStorageProvider storageProvider) {
            _contentManager = contentManager;
            _storageProvider = storageProvider;
        }

        public string UploadAttachment(HttpPostedFileBase file) {
            var extension = Path.GetExtension(file.FileName);
            var temporaryFileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
            var path = TempFolderPath + "/" + temporaryFileName;
            _storageProvider.SaveStream(path, file.InputStream);
            return temporaryFileName;
        }

        public void AssociateAttachments(IContent content, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames) {
            if (uploadedFileNames == null || originalFileNames == null)
                return;

            var folderPath = GetFolderPath(content);
            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();
            var currentAttachments = content.As<AttachmentsHolderPart>().Attachments.ToList();

            // Delete current attachments (unless they are part of the originalFileNames list).
            foreach (var attachment in currentAttachments.Where(x => !originalFiles.Contains(x.FileName, StringComparer.OrdinalIgnoreCase)).ToArray()) {
                _contentManager.Remove(attachment.ContentItem);
                currentAttachments.Remove(attachment);
            }

            _storageProvider.TryCreateFolder(folderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                if (String.IsNullOrEmpty(uploadedFiles[i]))
                    continue; // This file was already uploaded, we're just receiving back the filename.

                var uploadedFilePath = TempFolderPath + "/" + uploadedFiles[i];
                var originalFileName = Path.GetFileName(originalFiles[i]);
                var originalFilePath = folderPath + "/" + originalFileName;
                var attachment = _contentManager.Create<AttachmentPart>("Attachment", a => {
                    a.As<CommonPart>().Container = content;
                    a.FileName = originalFileName;
                    a.FileSize = _storageProvider.GetFile(uploadedFilePath).GetSize();
                });

                currentAttachments.Add(attachment);

                // Overwrite any existing files.
                DeleteFileFor(attachment);

                // Move the file from the temporary folder to the destination folder.
                _storageProvider.RenameFile(uploadedFilePath, originalFilePath);

            }

            content.As<AttachmentsHolderPart>().Attachments = currentAttachments;
        }

        private string GetFolderPath(IContent content) {
            return String.Format("_Attachments/{0:0000000}", content.Id);
        }

        public void DeleteFileFor(AttachmentPart attachment) {
            var filePath = GetFolderPath(attachment.As<ICommonPart>().Container) + "/" + attachment.FileName;

            if (_storageProvider.FileExists(filePath)) {
                _storageProvider.DeleteFile(filePath);
            }
        }

        public IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids) {
            return _contentManager.GetMany<AttachmentPart>(ids, VersionOptions.Published, QueryHints.Empty);
        }
    }
}
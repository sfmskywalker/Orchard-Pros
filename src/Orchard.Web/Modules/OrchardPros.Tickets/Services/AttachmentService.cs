using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.FileSystems.Media;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class AttachmentService : IAttachmentService {
        private readonly IContentManager _contentManager;
        private readonly IStorageProvider _storageProvider;

        public AttachmentService(IContentManager contentManager, IStorageProvider storageProvider) {
            _contentManager = contentManager;
            _storageProvider = storageProvider;
        }

        public string UploadAttachment(HttpPostedFileBase file) {
            var tempFolderPath = "_Attachments/_Temp";
            var extension = Path.GetExtension(file.FileName);
            var temporaryFileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
            var path = tempFolderPath + "/" + temporaryFileName;
            _storageProvider.SaveStream(path, file.InputStream);
            return temporaryFileName;
        }

        public void AssociateAttachments(IContent content, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames) {
            if (uploadedFileNames == null || originalFileNames == null)
                return;

            var tempFolderPath = "_Attachments/_Temp";
            var ticketFolderPath = String.Format("_Attachments/{0:0000000}", content.Id);
            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();
            var attachmentIds = content.As<AttachmentsHolderPart>().AttachmentIds.ToList();

            _storageProvider.TryCreateFolder(ticketFolderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                var uploadedFilePath = tempFolderPath + "/" + uploadedFiles[i];
                var originalFileName = Path.GetFileName(originalFiles[i]);
                var originalFilePath = ticketFolderPath + "/" + originalFileName;
                var attachment = _contentManager.Create<AttachmentPart>("Attachment", a => {
                    a.As<CommonPart>().Container = content;
                    a.FileName = originalFileName;
                });

                attachmentIds.Add(attachment.Id);
                _storageProvider.RenameFile(uploadedFilePath, originalFilePath);
            }

            content.As<AttachmentsHolderPart>().AttachmentIds = attachmentIds;
        }

        public IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids) {
            return _contentManager.GetMany<AttachmentPart>(ids, VersionOptions.Published, QueryHints.Empty);
        }
    }
}
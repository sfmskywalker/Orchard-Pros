using System.Collections.Generic;
using System.IO;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Services.Content {
    public interface IAttachmentService : IDependency {
        string UploadAttachment(HttpPostedFileBase file);
        void AssociateAttachments(IContent content, IEnumerable<string> uploadedFileNames, IList<string> uploadedFileContentTypes, IEnumerable<string> originalFileNames);
        void DeleteAttachments(IContent content);
        IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids);
        void DeleteFileFor(AttachmentPart attachment);
        AttachmentPart GetAttachment(int id);
        AttachmentPart GetAttachmentByIdentifier(string identifier);
        Stream OpenRead(AttachmentPart attachment);
    }
}
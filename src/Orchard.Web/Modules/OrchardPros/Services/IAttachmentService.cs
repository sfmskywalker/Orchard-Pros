using System.Collections.Generic;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface IAttachmentService : IDependency {
        string UploadAttachment(HttpPostedFileBase file);
        void AssociateAttachments(IContent content, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames);
        IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids);
        void DeleteFileFor(AttachmentPart attachment);
    }
}
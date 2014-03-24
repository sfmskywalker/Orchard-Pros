using System.Web.Mvc;
using Orchard.Themes;
using OrchardPros.Services.Content;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class AttachmentController : Controller {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService) {
            _attachmentService = attachmentService;
        }

        [HttpPost]
        public JsonResult Upload() {
            var file = Request.Files[0];
            var temporaryFileName = _attachmentService.UploadAttachment(file);

            return Json(new {
                uploadedFileName = temporaryFileName,
                uploadedFileContentType = file.ContentType
            });
        }

        public ActionResult Download(string id) {
            var attachment = _attachmentService.GetAttachmentByIdentifier(id);

            if (attachment == null)
                return HttpNotFound();

            attachment.DownloadCount++;
            var stream = _attachmentService.OpenRead(attachment);
            return File(stream, attachment.MimeType, attachment.OriginalFileName);
        }
    }
}
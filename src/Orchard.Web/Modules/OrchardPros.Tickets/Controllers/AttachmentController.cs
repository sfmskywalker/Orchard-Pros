using System.Web.Mvc;
using Orchard.Themes;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Controllers {
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
                uploadedFileName = temporaryFileName
            });
        }
    }
}
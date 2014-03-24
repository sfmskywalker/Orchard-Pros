using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Services.Content;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class ReplyController : Controller {
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly IReplyService _replyService;
        private readonly IAttachmentService _attachmentService;

        public ReplyController(IOrchardServices services, IReplyService replyService, IAttachmentService attachmentService) {
            _notifier = services.Notifier;
            _services = services;
            _replyService = replyService;
            _attachmentService = attachmentService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private IUser CurrentUser {
            get { return _services.WorkContext.CurrentUser; }
        }

        [HttpPost]
        public ActionResult Create(ReplyViewModel model) {
            var user = CurrentUser;

            if (!ModelState.IsValid) {
                return View(model);
            }

            var contentItem = _services.ContentManager.Get(model.ContentItemId);
            var reply = _replyService.Create(contentItem, model.Body, user, model.Title, model.ParentReplyId);

            if (model.Attachments != null)
                _attachmentService.AssociateAttachments(reply, model.Attachments.UploadedFileNames, model.Attachments.UploadedFileContentTypes, model.Attachments.OriginalFileNames);

            _notifier.Information(T("Your reply has been posted."));
            return Redirect(Url.ItemDisplayUrl(contentItem));
        }
    }
}
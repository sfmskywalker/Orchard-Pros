using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.JobsQueue.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Html;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Notify;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.ViewModels;

namespace OrchardPros.Controllers {
    [Authorize, Themed]
    public class ReplyController : Controller {
        private readonly INotifier _notifier;
        private readonly IOrchardServices _services;
        private readonly IReplyService _replyService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAuthorizer _authorizer;

        public ReplyController(IOrchardServices services, IReplyService replyService, IAttachmentService attachmentService) {
            _notifier = services.Notifier;
            _services = services;
            _replyService = replyService;
            _attachmentService = attachmentService;
            _authorizer = services.Authorizer;
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

        public ActionResult Edit(int id) {
            var reply = _services.ContentManager.Get<ReplyPart>(id);
            if(!_authorizer.Authorize(Permissions.EditOwnReply, reply))
                return new HttpUnauthorizedResult();

            var viewModel = new ReplyViewModel {
                Body = reply.Body,
                Title = reply.Subject,
                ContentItemId = reply.ContainingContent.Id,
                ParentReplyId = reply.ParentReplyId,
                Attachments = new AttachmentsViewModel {
                    CurrentFiles = reply.As<AttachmentsHolderPart>().Attachments.Select(x => new AttachmentViewModel { FileName = x.OriginalFileName, FileSize = x.FileSize }).ToList()
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(int id, ReplyViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var reply = _services.ContentManager.Get<ReplyPart>(id, VersionOptions.DraftRequired);
            if (!_authorizer.Authorize(Permissions.EditOwnReply, reply))
                return new HttpUnauthorizedResult();

            reply.Subject = model.Title.TrimSafe();
            reply.Body = model.Body.TrimSafe();

            if (model.Attachments != null)
                _attachmentService.AssociateAttachments(reply, model.Attachments.UploadedFileNames, model.Attachments.UploadedFileContentTypes, model.Attachments.OriginalFileNames);
            else {
                _attachmentService.DeleteAttachments(reply);
            }

            _services.ContentManager.Publish(reply.ContentItem);
            _notifier.Information(T("Your reply has been updated."));
            return Redirect(Url.ItemDisplayUrl(reply.ContainingContent));
        }
    }
}
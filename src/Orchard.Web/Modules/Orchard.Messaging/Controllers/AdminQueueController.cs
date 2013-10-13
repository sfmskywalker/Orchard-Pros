using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Messaging.Models;
using Orchard.Messaging.Services;
using Orchard.Messaging.ViewModels;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;

namespace Orchard.Messaging.Controllers {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    [Admin]
    public class AdminQueueController : Controller {
        private readonly IMessageQueueManager _messageQueueManager;
        private readonly IOrchardServices _services;

        public AdminQueueController(IMessageQueueManager messageQueueManager, IOrchardServices services) {
            _messageQueueManager = messageQueueManager;
            _services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ActionResult Index() {
            var queues = _messageQueueManager.GetQueues().ToList();

            if (queues.Count == 1)
                return RedirectToAction("List", new {id = queues.First().Id});

            var queueShapes = queues.Select(x => _services.New.Queue(x)
                .Pending(_messageQueueManager.CountMessages(x.Id, QueuedMessageStatus.Pending))
                .Faulted(_messageQueueManager.CountMessages(x.Id, QueuedMessageStatus.Faulted))
                .Sent(_messageQueueManager.CountMessages(x.Id, QueuedMessageStatus.Sent))).ToList();

            var model = _services.New.ViewModel()
                .Queues(queueShapes);

            return View(model);
        }

        public ActionResult Edit(int id, string returnUrl) {
            var queue = _messageQueueManager.GetQueue(id);
            var model = new MessageQueueViewModel {
                Id = queue.Id,
                Name = queue.Name,
                UpdateFrequency = queue.UpdateFrequency,
                TimeSlice = queue.TimeSlice,
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MessageQueueViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            CreateOrUpdateQueue(model);
            _services.Notifier.Information(T("Your queue has been updated."));
            return Url.IsLocalUrl(model.ReturnUrl) ? (ActionResult) Redirect(model.ReturnUrl) : RedirectToAction("Edit", new {id = model.Id});
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MessageQueueViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            var queue = CreateOrUpdateQueue(model);
            _services.Notifier.Information(T("Your queue has been created."));
            return RedirectToAction("Edit", new { id = queue.Id });
        }

        public ActionResult List(int id, MessagesFilter filter, PagerParameters pagerParameters) {
            var pager = new Pager(_services.WorkContext.CurrentSite, pagerParameters);
            var queue = _messageQueueManager.GetQueue(id);

            if (queue == null)
                return HttpNotFound();

            var messageCount = _messageQueueManager.CountMessages(queue.Id, filter.Status);
            var messages = _messageQueueManager.GetMessages(queue.Id, filter.Status, pager.GetStartIndex(), pager.PageSize).ToList();
            var model = _services.New.ViewModel()
                .Pager(_services.New.Pager(pager).TotalItemCount(messageCount))
                .Queue(queue)
                .Messages(messages)
                .Filter(filter);

            return View(model);
        }

        private MessageQueue CreateOrUpdateQueue(MessageQueueViewModel model) {
            var queue = _messageQueueManager.GetQueue(model.Id) ?? _messageQueueManager.CreateQueue();
            queue.Name = model.Name;
            queue.UpdateFrequency = model.UpdateFrequency;
            queue.TimeSlice = model.TimeSlice;
            return queue;
        }
    }
}
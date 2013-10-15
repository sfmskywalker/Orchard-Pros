using System.Web.Mvc;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Messaging.Services;
using Orchard.UI.Admin;

namespace Orchard.Messaging.Controllers {
    [OrchardFeature("Orchard.Messaging.Queuing")]
    [Admin]
    public class AdminPriorityController : Controller {
        private readonly IMessageQueueManager _messageQueueManager;

        public AdminPriorityController(IMessageQueueManager messageQueueManager, IShapeFactory shapeFactory) {
            _messageQueueManager = messageQueueManager;
            New = shapeFactory;
        }

        public dynamic New { get; set; }

        public ActionResult Index() {
            var model = New.ViewModel();
            return View(model);
        }

    }
}
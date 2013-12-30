using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OrchardPros.Careers.Models;
using OrchardPros.Careers.Services;

namespace OrchardPros.Careers.Controllers {
    [Admin]
    public class RecommendationController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly INotifier _notifier;

        public RecommendationController(IRecommendationManager recommendationManager, IOrchardServices services) {
            _contentManager = services.ContentManager;
            _recommendationManager = recommendationManager;
            _notifier = services.Notifier;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpPost]
        public ActionResult Approve(int id) {
            var recommendation = _contentManager.Get<RecommendationPart>(id);
            var profile = recommendation.User;
            _recommendationManager.Approve(recommendation);
            _notifier.Information(T("That Recommendation has been approved."));
            return RedirectToAction("Edit", "Admin", new { profile.Id, Area = "Orchard.Users" });
        }
    }
}
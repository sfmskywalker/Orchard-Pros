using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class RecommendationPartHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        private readonly IUserManager _userManager;
        private readonly IEmailService _emailService;

        public RecommendationPartHandler(IRepository<RecommendationPartRecord> repository, IContentManager contentManager, IUserManager userManager, IEmailService emailService) {
            _contentManager = contentManager;
            _userManager = userManager;
            _emailService = emailService;
            T = NullLocalizer.Instance;

            Filters.Add(StorageFilter.For(repository));
            OnActivated<RecommendationPart>(SetupLazyFields);
            OnCreated<RecommendationPart>(AddActivityPoints);
            OnCreated<RecommendationPart>(SendNotifications);
        }

        public Localizer T { get; set; }

        private void AddActivityPoints(CreateContentContext context, RecommendationPart part) {
            _userManager.AddActivityPoints(part.RecommendingUser, 2);
        }

        private void SendNotifications(CreateContentContext context, RecommendationPart part) {
            var recommendedUser = _contentManager.Get<IUser>(part.RecommendedUserId);
            _emailService.Queue(T("You have been recommended!"), recommendedUser.Email, "Template_Notification_NewRecommendation", new {
                RecommendedUser = recommendedUser,
                RecommendingUser = part.RecommendingUser,
                Recommendation = part
            });
        }

        private void SetupLazyFields(ActivatedContentContext context, RecommendationPart part) {
            part.UserField.Loader(() => _contentManager.Get<IUser>(part.RecommendedUserId));
            part.UserField.Setter(x => {
                part.RecommendedUserId = x.Id;
                return x;
            });
        }
    }
}
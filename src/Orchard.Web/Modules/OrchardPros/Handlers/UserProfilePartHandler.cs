using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using OrchardPros.Models;
using OrchardPros.Services;

namespace OrchardPros.Handlers {
    public class UserProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly IExperienceManager _experienceManager;
        private readonly IReplyService _replyService;
        private readonly ITicketService _ticketService;

        public UserProfilePartHandler(
            IRepository<UserProfilePartRecord> repository,
            IPositionManager positionManager, 
            ISkillManager skillManager, 
            IRecommendationManager recommendationManager, 
            IExperienceManager experienceManager, 
            IReplyService replyService, 
            ITicketService ticketService) {

            _positionManager = positionManager;
            _skillManager = skillManager;
            _recommendationManager = recommendationManager;
            _experienceManager = experienceManager;
            _replyService = replyService;
            _ticketService = ticketService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<UserProfilePart>(SetupFields);
        }

        private void SetupFields(ActivatedContentContext context, UserProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToArray());
            part.SkillsField.Loader(() => _skillManager.Fetch(part.Id).ToArray());
            part.RecommendationsField.Loader(() => _recommendationManager.GetByUser(part.Id).List<RecommendationPart>().ToArray());
            part.ExperienceField.Loader(() => _experienceManager.Fetch(part.Id).ToArray());
            part.RepliesField.Loader(() => _replyService.GetRepliesByUser(part.Id).ToArray());
            part.SolvedTicketsField.Loader(() => _ticketService.GetSolvedTicketsFor(part.Id).ToArray());
        }
    }
}
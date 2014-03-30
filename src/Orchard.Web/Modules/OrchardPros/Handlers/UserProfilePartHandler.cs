using System.Linq;
using Contrib.Voting.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Security;
using OrchardPros.Models;
using OrchardPros.Services.Content;
using OrchardPros.Services.User;

namespace OrchardPros.Handlers {
    public class UserProfilePartHandler : ContentHandler {
        private readonly IPositionManager _positionManager;
        private readonly ISkillManager _skillManager;
        private readonly IRecommendationManager _recommendationManager;
        private readonly IExperienceManager _experienceManager;
        private readonly IReplyService _replyService;
        private readonly ITicketService _ticketService;
        private readonly IVotingService _votingService;

        public UserProfilePartHandler(
            IRepository<UserProfilePartRecord> repository,
            IPositionManager positionManager, 
            ISkillManager skillManager, 
            IRecommendationManager recommendationManager, 
            IExperienceManager experienceManager, 
            IReplyService replyService, 
            ITicketService ticketService, IVotingService votingService) {

            _positionManager = positionManager;
            _skillManager = skillManager;
            _recommendationManager = recommendationManager;
            _experienceManager = experienceManager;
            _replyService = replyService;
            _ticketService = ticketService;
            _votingService = votingService;
            Filters.Add(StorageFilter.For(repository));
            OnActivated<UserProfilePart>(SetupFields);
            OnIndexing<UserProfilePart>(IndexUserProfiles);
        }

        private void SetupFields(ActivatedContentContext context, UserProfilePart part) {
            part.PositionsField.Loader(() => _positionManager.Fetch(part.Id).ToArray());
            part.SkillsField.Loader(() => _skillManager.Fetch(part.Id).ToArray());
            part.RecommendationsField.Loader(() => _recommendationManager.GetByRecommendedUser(part.Id).List<RecommendationPart>().ToArray());
            part.ExperienceField.Loader(() => _experienceManager.Fetch(part.Id).ToArray());
            part.RepliesField.Loader(() => _replyService.GetRepliesByUser(part.Id).Count());
            part.SolvedTicketsField.Loader(() => _ticketService.GetTicketsSolvedBy(part.Id, 0, 1).TotalItemCount);
            part.RatingField.Loader(() => {
                var result = _votingService.GetResult(part.Id, "average");
                return (int) (result != null ? result.Value : 1);
            });
        }

        private void IndexUserProfiles(IndexContentContext context, UserProfilePart part) {
            context.DocumentIndex
                .Add("username", part.As<IUser>().UserName).Store()
                .Add("firstname", part.FirstName).Store()
                .Add("lastname", part.LastName).Store()
                .Add("bio", part.Bio).RemoveTags().Analyze().Store();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Security;
using Orchard.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using OrchardPros.Helpers;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public class TicketService : ITicketService {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;
        private readonly IExperienceCalculator _experienceCalculator;
        private readonly ISignals _signals;
        private readonly IClock _clock;
        private readonly IReplyService _replyService;
        private readonly IRepository<TicketPartRecord> _ticketPartRepository;

        public TicketService(
            ITaxonomyService taxonomyService,
            IContentManager contentManager,
            IExperienceCalculator experienceCalculator,
            ISignals signals,
            IClock clock,
            IReplyService replyService,
            IRepository<TicketPartRecord> ticketPartRepository) {

            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _experienceCalculator = experienceCalculator;
            _signals = signals;
            _clock = clock;
            _replyService = replyService;
            _ticketPartRepository = ticketPartRepository;
        }

        public IPagedList<TicketPart> GetTicketsFor(int userId, int? skip = null, int? take = null) {
            var query = _contentManager.Query<CommonPart, CommonPartRecord>().Where(x => x.OwnerId == userId).Join<TicketPartRecord>();
            var totalItemCount = query.Count();

            if (skip != null && take != null) {
                return new PagedList<TicketPart>(query.Slice(skip.Value, take.Value).Select(x => x.As<TicketPart>()), totalItemCount);
            }

            return new PagedList<TicketPart>(query.ForPart<TicketPart>().List(), totalItemCount);
        }

        public IEnumerable<TermPart> GetCategories() {
            return GetTerms("Category");
        }

        public IEnumerable<TermPart> GetCategoriesFor(int ticketId) {
            return _taxonomyService.GetTermsForContentItem(ticketId, "Categories");
        }

        public IEnumerable<TermPart> GetTags() {
            return GetTerms("Tag");
        }

        public IEnumerable<TermPart> GetTagsFor(int ticketId) {
            return _taxonomyService.GetTermsForContentItem(ticketId, "Tags");
        }

        public TicketPart Create(UserProfilePart user, string subject, string body, TicketType type = TicketType.Question, Action<TicketPart> initialize = null) {
            return _contentManager.Create<TicketPart>("Ticket", VersionOptions.Published, t => {
                t.User = user.As<IUser>();
                t.Subject = subject;
                t.Body = body;

                if (initialize != null)
                    initialize(t);
            });
        }

        public int CalculateExperience(UserProfilePart user) {
            return _experienceCalculator.CalculateForTicket(user);
        }

        public TimeSpan GetRemainingTimeFor(TicketPart ticket) {
            var timeSpan = ticket.DeadlineUtc - _clock.UtcNow;
            return timeSpan.Ticks < 0 ? TimeSpan.Zero : timeSpan;
        }

        public TicketPart GetTicket(int id) {
            return _contentManager.Get<TicketPart>(id);
        }

        public void AssignCategories(TicketPart ticket, IEnumerable<int> categoryIds) {
            var categoryList = (categoryIds ?? Enumerable.Empty<int>()).ToArray();
            var terms = _contentManager.GetMany<TermPart>(categoryList, VersionOptions.Published, QueryHints.Empty);
            _taxonomyService.UpdateTerms(ticket.ContentItem, terms, "Categories");
        }

        public void AssignTags(TicketPart ticket, string tags) {
            var tagList = !String.IsNullOrWhiteSpace(tags) ? ParseTags(tags).Select(x => x.Id) : Enumerable.Empty<int>();
            var terms = _contentManager.GetMany<TermPart>(tagList, VersionOptions.Published, QueryHints.Empty);
            _taxonomyService.UpdateTerms(ticket.ContentItem, terms, "Tags");
        }

        public IPagedList<TicketPart> GetTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest, int? categoryId = null, int? tagId = null) {
            var baseQuery = _contentManager.Query<TicketPart, TicketPartRecord>(VersionOptions.Published);
            IContentQuery<TicketPart, CommonPartRecord> commonQuery;

            switch (criteria) {
                case TicketsCriteria.Unsolved:
                    commonQuery = baseQuery.Where<TicketPartRecord>(x => x.SolvedUtc == null).Join<CommonPartRecord>().OrderByDescending(x => x.CreatedUtc);
                    break;
                case TicketsCriteria.Popular:
                    commonQuery = baseQuery.OrderByDescending<StatisticsPartRecord>(x => x.ViewCount).OrderByDescending<CommonPartRecord>(x => x.CreatedUtc);
                    break;
                case TicketsCriteria.Deadline:
                    commonQuery = baseQuery.OrderBy<TicketPartRecord>(x => x.DeadlineUtc).Join<CommonPartRecord>();
                    break;
                case TicketsCriteria.Bounty:
                    commonQuery = baseQuery.Where<TicketPartRecord>(x => x.Bounty == null).OrderByDescending(x => x.Bounty).Join<CommonPartRecord>();
                    break;
                default:
                    commonQuery = baseQuery.OrderByDescending<CommonPartRecord>(x => x.CreatedUtc).Join<CommonPartRecord>();
                    break;
            }

            if (categoryId != null) {
                var category = String.Format("|{0}|", categoryId);
                commonQuery = baseQuery.Where<TicketPartRecord>(x => x.Categories.Contains(category)).Join<CommonPartRecord>();
            }

            if (tagId != null) {
                var tag = String.Format("|{0}|", tagId);
                commonQuery = baseQuery.Where<TicketPartRecord>(x => x.Tags.Contains(tag)).Join<CommonPartRecord>();
            }

            var ticketsQuery = commonQuery;
            var pagedQuery = skip != null && take != null ? ticketsQuery.Slice(skip.Value, take.Value) : ticketsQuery.List();
            var tickets = pagedQuery.ToArray();
            var totalCount = skip == null || take == null ? tickets.Length : ticketsQuery.List().Count();

            return tickets.ToPagedList(totalCount);
        }

        public DateTime? GetLastModifiedUtcFor(TicketPart ticket) {
            var lastReply = ticket.Replies.LastOrDefault();
            var ticketModifiedUtc = ticket.As<CommonPart>().ModifiedUtc ?? DateTime.MinValue;
            var replyModifiedUtc = (lastReply != null ? lastReply.As<CommonPart>().ModifiedUtc : default(DateTime?)) ?? DateTime.MinValue;
            var timeStamp = replyModifiedUtc > ticketModifiedUtc ? replyModifiedUtc : ticketModifiedUtc;
            return timeStamp == DateTime.MinValue ? default(DateTime?) : timeStamp;
        }

        public IUser GetLastModifierFor(TicketPart ticket) {
            var lastReply = ticket.Replies.LastOrDefault();
            var ticketModifiedUtc = ticket.As<CommonPart>().ModifiedUtc ?? DateTime.MinValue;
            var replyModifiedUtc = (lastReply != null ? lastReply.As<CommonPart>().ModifiedUtc : default(DateTime?)) ?? DateTime.MinValue;
            var user = (replyModifiedUtc > ticketModifiedUtc) && lastReply != null ? lastReply.User : ticket.User;
            return user;
        }

        public void Publish(TicketPart ticket) {
            _contentManager.Publish(ticket.ContentItem);
        }

        public void Solve(TicketPart ticket, ReplyPart reply) {
            if (ticket.SolvedUtc != null)
                throw new InvalidOperationException("The ticket has already been solved");

            var expertPart = reply.User.As<UserProfilePart>();

            ticket.SolvedUtc = _clock.UtcNow;
            ticket.AnswerId = reply.Id;
            expertPart.ExperiencePoints += ticket.ExperiencePoints;

            // TODO: Add to activity stream

            if (ticket.Bounty != null) {
                // TODO: Transfer funds.
            }
        }

        public IEnumerable<TicketPart> GetSolvedTicketsFor(int userId) {
            var replyIds = _replyService.GetRepliesByUser(userId).Select(x => x.Id).ToArray();
            var answeredTicketsQuery = from ticket in _ticketPartRepository.Table
                                       where ticket.AnswerId != null
                                       let answerId = ticket.AnswerId.Value
                                       where replyIds.Contains(answerId)
                                       select ticket.Id;
            var ticketIds = answeredTicketsQuery.ToArray();
            return _contentManager.GetMany<TicketPart>(ticketIds, VersionOptions.Published, QueryHints.Empty);
        }

        private IEnumerable<TermPart> ParseTags(string tags) {
            var tagList = !String.IsNullOrWhiteSpace(tags) ? tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLower()).ToArray() : new string[0];
            var taxonomy = GetOrCreateTaxonomy("Tag");
            var termsDictionary = GetTerms(taxonomy.Id).ToDictionary(x => x.Name.ToLower());

            foreach (var tag in tagList) {
                if (termsDictionary.ContainsKey(tag)) {
                    yield return termsDictionary[tag];
                    continue;
                }

                var term = CreateTerm(taxonomy, tag);
                _signals.Trigger(Signals.TagDictionary);
                termsDictionary.Add(tag, term);
                yield return term;
            }
        }

        private TermPart CreateTerm(TaxonomyPart taxonomy, string name) {
            var term = _taxonomyService.NewTerm(taxonomy);
            term.Container = taxonomy;
            term.Name = name;
            _taxonomyService.ProcessPath(term);
            _contentManager.Create(term, VersionOptions.Published);
            return term;
        }

        private TaxonomyPart GetOrCreateTaxonomy(string taxonomyName) {
            return _taxonomyService.GetTaxonomyByName(taxonomyName) ?? _contentManager.Create<TaxonomyPart>("Taxonomy", VersionOptions.Published, part => {
                part.Name = taxonomyName;
                part.IsInternal = false;
            });
        }

        private IEnumerable<TermPart> GetTerms(string taxonomyName) {
            var taxonomy = GetOrCreateTaxonomy(taxonomyName);
            return GetTerms(taxonomy.Id);
        }

        private IEnumerable<TermPart> GetTerms(int taxonomyId) {
            return _taxonomyService.GetTerms(taxonomyId).OrderBy(x => x.Name);
        }
    }
}
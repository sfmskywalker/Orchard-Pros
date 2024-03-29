using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Search.Services;
using Orchard.Security;
using Orchard.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using OrchardPros.Helpers;
using OrchardPros.Models;
using OrchardPros.Services.Commerce;
using OrchardPros.Services.User;

namespace OrchardPros.Services.Content {
    public class TicketService : ITicketService {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;
        private readonly IUserManager _userManager;
        private readonly ISignals _signals;
        private readonly IClock _clock;
        private readonly ISearchService _searchService;
        private readonly ITicketEventHandler _ticketEventHandlers;
        private readonly ITransferService _transferService;

        public TicketService(
            ITaxonomyService taxonomyService,
            IContentManager contentManager,
            IUserManager userManager,
            ISignals signals,
            IClock clock,
            ISearchService searchService, 
            ITicketEventHandler ticketEventHandlers, 
            ITransferService transferService) {

            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _userManager = userManager;
            _signals = signals;
            _clock = clock;
            _searchService = searchService;
            _ticketEventHandlers = ticketEventHandlers;
            _transferService = transferService;
        }

        public IPagedList<TicketPart> GetTicketsCreatedBy(int userId, int? skip = null, int? take = null) {
            var query = _contentManager.Query<CommonPart, CommonPartRecord>().Where(x => x.OwnerId == userId).Join<TicketPartRecord>();
            var totalItemCount = query.Count();

            if (skip != null && take != null) {
                return new PagedList<TicketPart>(query.Slice(skip.Value, take.Value).Select(x => x.As<TicketPart>()), totalItemCount);
            }

            return new PagedList<TicketPart>(query.ForPart<TicketPart>().List(), totalItemCount);
        }

        public IPagedList<TicketPart> GetTicketsSolvedBy(int userId, int? skip = null, int? take = null) {
            var query = _contentManager.Query<TicketPart, TicketPartRecord>().Where(x => x.SolvedByUserId == userId);
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

        public TicketPart Create(IUser user, string subject, string body, TicketType type = TicketType.Question, Action<TicketPart> initialize = null) {
            return _contentManager.Create<TicketPart>("Ticket", VersionOptions.Published, t => {
                t.User = user;
                t.Subject = subject;
                t.Body = body;

                if (initialize != null)
                    initialize(t);
            });
        }

        public int CalculateExperience(IUser user) {
            return _userManager.CalculateXpWhenSolved(user);
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

        public IPagedList<TicketPart> GetTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest, int? categoryId = null, int? tagId = null, string term = null) {
            var baseQuery = _contentManager.Query<TicketPart>(VersionOptions.Published);

            if (!String.IsNullOrWhiteSpace(term)) {
                var foundTicketIds = SearchTickets(term, skip.GetValueOrDefault(), take);
                baseQuery = baseQuery.ForContentItems(foundTicketIds);
            }

            baseQuery = baseQuery.Join<TicketPartRecord>();
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
                    commonQuery = baseQuery.Where<TicketPartRecord>(x => x.Bounty != null).OrderByDescending(x => x.Bounty).Join<CommonPartRecord>();
                    break;
                default:
                    commonQuery = baseQuery.OrderByDescending<CommonPartRecord>(x => x.CreatedUtc).Join<CommonPartRecord>();
                    break;
            }

            if (categoryId != null) {
                var category = String.Format("|{0}|", categoryId);
                commonQuery = commonQuery.Where<TicketPartRecord>(x => x.Categories.Contains(category)).Join<CommonPartRecord>();
            }

            if (tagId != null) {
                var tag = String.Format("|{0}|", tagId);
                commonQuery = commonQuery.Where<TicketPartRecord>(x => x.Tags.Contains(tag)).Join<CommonPartRecord>();
            }

            var ticketsQuery = commonQuery;
            var totalCount = ticketsQuery.Count();
            var pagedQuery = skip != null && take != null ? ticketsQuery.Slice(skip.Value, take.Value) : ticketsQuery.List();
            var tickets = pagedQuery.ToArray();

            return tickets.ToPagedList(totalCount);
        }

        public DateTime? GetLastModifiedUtcFor(TicketPart ticket) {
            var lastReply = ticket.As<RepliesPart>().Replies.LastOrDefault();
            var ticketModifiedUtc = ticket.As<CommonPart>().ModifiedUtc ?? DateTime.MinValue;
            var replyModifiedUtc = (lastReply != null ? lastReply.As<CommonPart>().ModifiedUtc : default(DateTime?)) ?? DateTime.MinValue;
            var timeStamp = replyModifiedUtc > ticketModifiedUtc ? replyModifiedUtc : ticketModifiedUtc;
            return timeStamp == DateTime.MinValue ? default(DateTime?) : timeStamp;
        }

        public IUser GetLastModifierFor(TicketPart ticket) {
            var lastReply = ticket.As<RepliesPart>().Replies.LastOrDefault();
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

            ticket.SolvedUtc = _clock.UtcNow;
            ticket.SolvedByUserId = reply.User.Id;
            ticket.AnswerId = reply.Id;

            _ticketEventHandlers.Solved(new TicketSolvedContext {
                Ticket = ticket,
                Expert = reply.User
            });

            // TODO: Add to activity stream

            if (ticket.Bounty != null) {
                _transferService.Create(reply.User.Id, ticket.Bounty.Value, "USD", ticket.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        public IEnumerable<TermPart> GetPopularTags() {
            return GetTags().OrderByDescending(x => x.Weight).Take(15).OrderBy(x => x.Name);
        }

        private IEnumerable<int> SearchTickets(string term, int skip, int? take) {
            var searchHits = _searchService.Query(
                query: term,
                filterCulture: false,
                page: skip,
                pageSize: take,
                index: "Tickets",
                searchFields: new[] { "author", "body", "title", "Categories", "Tags" },
                shapeResult: searchHit => searchHit);

            var foundIds = searchHits.Select(searchHit => searchHit.ContentItemId).ToList();
            return foundIds;
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
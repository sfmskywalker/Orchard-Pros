using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NHibernate.Transform;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.FileSystems.Media;
using Orchard.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.Users.Models;
using OrchardPros.Tickets.Helpers;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class TicketService : ITicketService {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;
        private readonly IClock _clock;
        private readonly IExperienceCalculator _experienceCalculator;
        private readonly ICacheManager _cache;
        private readonly ISignals _signals;
        private readonly IRepository<TicketCategory> _ticketCategoryRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly ISessionLocator _sessionLocator;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IRepository<TicketTag> _ticketTagRepository;

        public TicketService(
            IRepository<Ticket> ticketRepository, 
            ITaxonomyService taxonomyService, 
            IContentManager contentManager, 
            IClock clock, 
            IExperienceCalculator experienceCalculator, 
            ICacheManager cache, 
            ISignals signals, 
            IRepository<TicketCategory> ticketCategoryRepository,
            IStorageProvider storageProvider, 
            ISessionLocator sessionLocator, 
            IRepository<Attachment> attachmentRepository, 
            IRepository<TicketTag> ticketTagRepository) {

            _ticketRepository = ticketRepository;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _clock = clock;
            _experienceCalculator = experienceCalculator;
            _cache = cache;
            _signals = signals;
            _ticketCategoryRepository = ticketCategoryRepository;
            _storageProvider = storageProvider;
            _sessionLocator = sessionLocator;
            _attachmentRepository = attachmentRepository;
            _ticketTagRepository = ticketTagRepository;
        }

        public IEnumerable<Ticket> GetTicketsFor(int userId) {
            return _ticketRepository.Table.Where(x => x.UserId == userId && x.ArchivedUtc == null);
        }

        public IEnumerable<TermPart> GetCategories() {
            return GetTerms("Category");
        }

        public IEnumerable<TermPart> GetTags() {
            return GetTerms("Tag");
        }

        public IEnumerable<TermPart> GetTagsFor(Ticket ticket) {
            var termIds = ticket.Tags.Select(x => x.TagId).ToArray();
            return _contentManager.GetMany<TermPart>(termIds, VersionOptions.Latest, QueryHints.Empty);
        }

        public Ticket Create(ExpertPart user, string title, string description, TicketType type = TicketType.Question, Action<Ticket> initialize = null) {
            var ticket = new Ticket {
                UserId = user.Id,
                Title = title,
                Description = description,
                CreatedUtc = _clock.UtcNow,
                LastModifiedUtc = _clock.UtcNow
            };

            if (initialize != null)
                initialize(ticket);

            _ticketRepository.Create(ticket);
            return ticket;
        }

        public int CalculateExperience(ExpertPart user) {
            return _experienceCalculator.CalculateForTicket(user);
        }

        public Ticket GetTicket(int id) {
            return _ticketRepository.Get(id);
        }

        public IDictionary<int, string> GetCategoryDictionary() {
            return _cache.Get("CategoryDictionary", context => {
                context.Monitor(_signals.When(Signals.CategoryDictionary));
                return GetCategories().ToDictionary(x => x.Id, x => x.Name);
            });
        }

        public IDictionary<int, string> GetTagDictionary() {
            return _cache.Get("TagDictionary", context => {
                context.Monitor(_signals.When(Signals.TagDictionary));
                return GetTags().ToDictionary(x => x.Id, x => x.Name);
            });
        }

        public void Archive(Ticket ticket) {
            ticket.ArchivedUtc = _clock.UtcNow;
        }

        public void AssignCategories(Ticket ticket, IEnumerable<int> categoryIds) {
            var categoryList = (categoryIds ?? Enumerable.Empty<int>()).ToArray();

            // Delete current categories
            foreach (var category in ticket.Categories.Where(x => !categoryList.Contains(x.CategoryId)).ToArray()) {
                ticket.Categories.Remove(category);
                _ticketCategoryRepository.Delete(category);
            }

            // Add new categories
            var existingCategoryIds = ticket.Categories.Select(x => x.CategoryId).ToArray();
            foreach (var categoryId in categoryList.Where(x => !existingCategoryIds.Contains(x))) {
                var category = new TicketCategory {Ticket = ticket, CategoryId = categoryId};
                _ticketCategoryRepository.Create(category);
                ticket.Categories.Add(category);
            }
        }

        public void AssignTags(Ticket ticket, string tags) {
            var tagList = !String.IsNullOrWhiteSpace(tags) ? ParseTags(tags).Select(x => x.Id) : Enumerable.Empty<int>();

            // Delete current tags
            foreach (var tag in ticket.Tags.Where(x => !tagList.Contains(x.TagId)).ToArray()) {
                ticket.Tags.Remove(tag);
                _ticketTagRepository.Delete(tag);
            }

            // Add new categories
            var existingTagIds = ticket.Tags.Select(x => x.TagId).ToArray();
            foreach (var tagId in tagList.Where(x => !existingTagIds.Contains(x))) {
                var tag = new TicketTag { Ticket = ticket, TagId = tagId };
                _ticketTagRepository.Create(tag);
                ticket.Tags.Add(tag);
            }
        }

        public string UploadAttachment(HttpPostedFileBase file) {
            var tempFolderPath = "_Attachments/_Temp";
            var extension = Path.GetExtension(file.FileName);
            var temporaryFileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
            var path = tempFolderPath + "/" + temporaryFileName;
            _storageProvider.SaveStream(path, file.InputStream);
            return temporaryFileName;
        }

        public void AssociateAttachments(Ticket ticket, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames) {
            if (uploadedFileNames == null || originalFileNames == null)
                return;

            var tempFolderPath = "_Attachments/_Temp";
            var ticketFolderPath = String.Format("_Attachments/{0:0000000}", ticket.Id);
            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();

            _storageProvider.TryCreateFolder(ticketFolderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                var uploadedFilePath = tempFolderPath + "/"  + uploadedFiles[i];
                var originalFileName = Path.GetFileName(originalFiles[i]);
                var originalFilePath = ticketFolderPath + "/" + originalFileName;
                var attachment = new Attachment {
                    Ticket = ticket,
                    CreatedUtc = _clock.UtcNow,
                    FileName = originalFileName
                };

                _attachmentRepository.Create(attachment);
                ticket.Attachments.Add(attachment);
                _storageProvider.RenameFile(uploadedFilePath, originalFilePath);
            }
        }

        public IPagedList<TicketSummary> GetSummarizedTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest) {
            var session = _sessionLocator.For(typeof (Ticket));
            var baseQuery = session.QueryOver<Ticket>();
            var categoryDictionary = GetCategoryDictionary();
            var tagDictionary = GetTagDictionary();

            baseQuery.RootCriteria.SetResultTransformer(new DistinctRootEntityResultTransformer());

            switch (criteria) {
                case TicketsCriteria.Unsolved:
                    baseQuery = baseQuery.Where(x => x.SolvedUtc == null).OrderBy(x => x.CreatedUtc).Desc;
                    break;
                case TicketsCriteria.Popular:
                    baseQuery = baseQuery.OrderBy(x => x.ViewCount).Desc.ThenBy(x => x.CreatedUtc).Desc;
                    break;
                case TicketsCriteria.Deadline:
                    baseQuery = baseQuery.OrderBy(x => x.DeadlineUtc).Asc;
                    break;
                case TicketsCriteria.Bounty:
                    baseQuery = baseQuery.WhereNot(x => x.Bounty == null).OrderBy(x => x.Bounty).Desc;
                    break;
                default:
                    baseQuery = baseQuery.OrderBy(x => x.CreatedUtc).Desc;
                    break;
            }

            var ticketsQuery = baseQuery;
            var pagedQuery = skip != null && take != null ? ticketsQuery.Skip(skip.Value).Take(take.Value) : baseQuery;
            var tickets = pagedQuery.Future().ToArray();
            var totalCount = skip == null || take == null ? tickets.Length : baseQuery.RowCount();
            var userIds = CollectUserIds(tickets).ToArray();
            var userDictionary = session.QueryOver<UserPartRecord>().WhereRestrictionOn(x => x.Id).IsIn(userIds).Future().ToDictionary(x => x.Id, x => x.UserName);

            return tickets.Select(ticket => new TicketSummary {
                Id = ticket.Id,
                Bounty = ticket.Bounty,
                Categories = ticket.Categories.Sanitize(categoryDictionary).ToDictionary(x => x.CategoryId, x => categoryDictionary[x.CategoryId]),
                Tags = ticket.Tags.Sanitize(tagDictionary).ToDictionary(x => x.TagId, x => tagDictionary[x.TagId]),
                CreatedUtc = ticket.CreatedUtc,
                DeadlineUtc = ticket.DeadlineUtc,
                TimeLeft = ticket.TimeLeft(),
                ExperiencePoints = ticket.ExperiencePoints,
                LastModifiedUtc = ticket.LastModifiedUtc,
                SolvedUtc = ticket.SolvedUtc,
                Title = ticket.Title,
                Type = ticket.Type,
                UserId = ticket.UserId,
                UserName = userDictionary[ticket.UserId],
                ViewCount = ticket.ViewCount,
                Replies = ticket.Replies.Select(x => new ReplySummary {
                    CreatedUtc = x.CreatedUtc,
                    UserId = x.UserId,
                    UserName = userDictionary[x.UserId]
                }).ToArray()
            }).ToPagedList(totalCount);
        }

        private IEnumerable<TermPart> ParseTags(string tags) {
            var tagList = !String.IsNullOrWhiteSpace(tags) ? tags.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLower()).ToArray() : new string[0];
            var taxonomy = GetOrCreateTaxonomy("Tag");
            var terms = GetTerms(taxonomy.Id).ToDictionary(x => x.Name.ToLower());

            foreach (var tag in tagList) {
                if (terms.ContainsKey(tag))
                    continue;
                
                var term = CreateTerm(taxonomy, tag);
                _signals.Trigger(Signals.TagDictionary);
                terms.Add(tag, term);
            }

            return terms.Values;
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

        private static IEnumerable<int> CollectUserIds(IEnumerable<Ticket> tickets) {
            foreach (var ticket in tickets) {
                yield return ticket.UserId;
                var lastReply = ticket.Replies.LastOrDefault();
                if (lastReply != null)
                    yield return lastReply.UserId;
            }
        }
    }
}
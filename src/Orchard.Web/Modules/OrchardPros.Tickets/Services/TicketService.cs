using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.FileSystems.Media;
using Orchard.Security;
using Orchard.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using OrchardPros.Tickets.Helpers;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public class TicketService : ITicketService {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;
        private readonly IExperienceCalculator _experienceCalculator;
        private readonly ICacheManager _cache;
        private readonly ISignals _signals;
        private readonly IStorageProvider _storageProvider;
        private readonly IClock _clock;

        public TicketService(
            ITaxonomyService taxonomyService, 
            IContentManager contentManager,
            IExperienceCalculator experienceCalculator, 
            ICacheManager cache, 
            ISignals signals, 
            IStorageProvider storageProvider, 
            IClock clock) {

            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _experienceCalculator = experienceCalculator;
            _cache = cache;
            _signals = signals;
            _storageProvider = storageProvider;
            _clock = clock;
        }

        public IEnumerable<TicketPart> GetTicketsFor(int userId) {
            return _contentManager.Query<CommonPart, CommonPartRecord>().Where(x => x.OwnerId == userId).List<TicketPart>();
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

        public TicketPart Create(ExpertPart user, string subject, string body, TicketType type = TicketType.Question, Action<TicketPart> initialize = null) {
            return _contentManager.Create<TicketPart>("Ticket", VersionOptions.Published, t => {
                t.User = user.As<IUser>();
                t.Subject = subject;
                t.Body = body;
                
                if (initialize != null)
                    initialize(t);
            });
        }

        public int CalculateExperience(ExpertPart user) {
            return _experienceCalculator.CalculateForTicket(user);
        }

        public TimeSpan GetRemainingTimeFor(TicketPart ticket) {
            var timeSpan = ticket.DeadlineUtc - _clock.UtcNow;
            return timeSpan.Ticks < 0 ? TimeSpan.Zero : timeSpan;
        }

        public TicketPart GetTicket(int id) {
            return _contentManager.Get<TicketPart>(id);
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

        public string UploadAttachment(HttpPostedFileBase file) {
            var tempFolderPath = "_Attachments/_Temp";
            var extension = Path.GetExtension(file.FileName);
            var temporaryFileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
            var path = tempFolderPath + "/" + temporaryFileName;
            _storageProvider.SaveStream(path, file.InputStream);
            return temporaryFileName;
        }

        public void AssociateAttachments(TicketPart ticket, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames) {
            if (uploadedFileNames == null || originalFileNames == null)
                return;

            var tempFolderPath = "_Attachments/_Temp";
            var ticketFolderPath = String.Format("_Attachments/{0:0000000}", ticket.Id);
            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();
            var attachmentIds = ticket.As<AttachmentsHolderPart>().AttachmentIds.ToList();

            _storageProvider.TryCreateFolder(ticketFolderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                var uploadedFilePath = tempFolderPath + "/"  + uploadedFiles[i];
                var originalFileName = Path.GetFileName(originalFiles[i]);
                var originalFilePath = ticketFolderPath + "/" + originalFileName;
                var attachment = _contentManager.Create<AttachmentPart>("Attachment", a => {
                    a.As<CommonPart>().Container = ticket;
                    a.FileName = originalFileName;
                });

                attachmentIds.Add(attachment.Id);
                _storageProvider.RenameFile(uploadedFilePath, originalFilePath);
            }

            ticket.As<AttachmentsHolderPart>().AttachmentIds = attachmentIds;
        }

        public IPagedList<TicketPart> GetTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest) {
            var baseQuery = _contentManager.Query();

            switch (criteria) {
                case TicketsCriteria.Unsolved:
                    baseQuery = baseQuery.Where<TicketPartRecord>(x => x.SolvedUtc == null).Join<CommonPartRecord>().OrderByDescending(x => x.CreatedUtc);
                    break;
                case TicketsCriteria.Popular:
                    baseQuery = baseQuery.OrderByDescending<StatisticsPartRecord>(x => x.ViewCount).OrderByDescending<CommonPartRecord>(x => x.CreatedUtc);
                    break;
                case TicketsCriteria.Deadline:
                    baseQuery = baseQuery.OrderBy<TicketPartRecord>(x => x.DeadlineUtc);
                    break;
                case TicketsCriteria.Bounty:
                    baseQuery = baseQuery.Where<TicketPartRecord>(x => x.Bounty == null).OrderByDescending(x => x.Bounty);
                    break;
                default:
                    baseQuery = baseQuery.OrderByDescending<CommonPartRecord>(x => x.CreatedUtc);
                    break;
            }

            var ticketsQuery = baseQuery;
            var pagedQuery = skip != null && take != null ? ticketsQuery.ForPart<TicketPart>().Slice(skip.Value, take.Value) : baseQuery.ForPart<TicketPart>().List();
            var tickets = pagedQuery.ToArray();
            var totalCount = skip == null || take == null ? tickets.Length : baseQuery.Count();

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

        private static IEnumerable<int> CollectUserIds(IEnumerable<TicketPart> tickets) {
            foreach (var ticket in tickets) {
                yield return ticket.User.Id;
                var lastReply = ticket.Replies.LastOrDefault();
                if (lastReply != null)
                    yield return lastReply.As<CommonPart>().Record.OwnerId;
            }
        }
    }
}
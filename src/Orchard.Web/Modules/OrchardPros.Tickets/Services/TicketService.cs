using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.FileSystems.Media;
using Orchard.Services;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
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

        public TicketService(
            IRepository<Ticket> ticketRepository, 
            ITaxonomyService taxonomyService, 
            IContentManager contentManager, 
            IClock clock, 
            IExperienceCalculator experienceCalculator, 
            ICacheManager cache, 
            ISignals signals, 
            IRepository<TicketCategory> ticketCategoryRepository,
            IStorageProvider storageProvider) {

            _ticketRepository = ticketRepository;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _clock = clock;
            _experienceCalculator = experienceCalculator;
            _cache = cache;
            _signals = signals;
            _ticketCategoryRepository = ticketCategoryRepository;
            _storageProvider = storageProvider;
        }

        public IEnumerable<Ticket> GetTicketsFor(int userId) {
            return _ticketRepository.Table.Where(x => x.UserId == userId && x.ArchivedUtc == null);
        }

        public IEnumerable<TermPart> GetCategories() {
            var categoryTaxonomy = _taxonomyService.GetTaxonomyByName("Category") ?? _contentManager.Create<TaxonomyPart>("Taxonomy", VersionOptions.Published, part => {
                part.Name = "Category";
                part.IsInternal = false;
            });

            return _taxonomyService.GetTerms(categoryTaxonomy.Id);
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

        public void Archive(Ticket ticket) {
            ticket.ArchivedUtc = _clock.UtcNow;
        }

        public IList<TicketCategory> AssignCategories(Ticket ticket, IEnumerable<int> categoryIds) {
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
            return ticket.Categories;
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
            var tempFolderPath = "_Attachments/_Temp";
            var ticketFolderPath = String.Format("_Attachments/{0:0000000}", ticket.Id);
            var uploadedFiles = uploadedFileNames.ToArray();
            var originalFiles = originalFileNames.ToArray();

            _storageProvider.TryCreateFolder(ticketFolderPath);

            for (var i = 0; i < uploadedFiles.Length; i++) {
                var uploadedFileName = tempFolderPath + "/"  + uploadedFiles[i];
                var originalFileName = ticketFolderPath + "/" + Path.GetFileName(originalFiles[i]);

                _storageProvider.RenameFile(uploadedFileName, originalFileName);
            }
        }
    }
}
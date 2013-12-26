using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Caching;
using Orchard.ContentManagement;
using Orchard.Data;
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

        public TicketService(
            IRepository<Ticket> ticketRepository, 
            ITaxonomyService taxonomyService, 
            IContentManager contentManager, 
            IClock clock, 
            IExperienceCalculator experienceCalculator, 
            ICacheManager cache, 
            ISignals signals, 
            IRepository<TicketCategory> ticketCategoryRepository) {

            _ticketRepository = ticketRepository;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _clock = clock;
            _experienceCalculator = experienceCalculator;
            _cache = cache;
            _signals = signals;
            _ticketCategoryRepository = ticketCategoryRepository;
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
            var categoryList = categoryIds.ToArray();

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
    }
}
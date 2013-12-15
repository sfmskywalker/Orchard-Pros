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

        public TicketService(
            IRepository<Ticket> ticketRepository, 
            ITaxonomyService taxonomyService, 
            IContentManager contentManager, 
            IClock clock, 
            IExperienceCalculator experienceCalculator, 
            ICacheManager cache, 
            ISignals signals) {

            _ticketRepository = ticketRepository;
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _clock = clock;
            _experienceCalculator = experienceCalculator;
            _cache = cache;
            _signals = signals;
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

        public Ticket Create(ExpertPart user, int categoryId, string title, string description, TicketType type = TicketType.Question, Action<Ticket> initialize = null) {
            var ticket = new Ticket {
                UserId = user.Id,
                Title = title,
                Description = description,
                CategoryId = categoryId,
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
    }
}
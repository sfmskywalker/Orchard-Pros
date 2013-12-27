using System;
using System.Collections.Generic;
using System.Web;
using Orchard;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface ITicketService : IDependency {
        IEnumerable<Ticket> GetTicketsFor(int userId);
        IEnumerable<TermPart> GetCategories();
        Ticket Create(ExpertPart user, string title, string description, TicketType type = TicketType.Question, Action<Ticket> initialize = null);
        int CalculateExperience(ExpertPart user);
        Ticket GetTicket(int id);
        IDictionary<int, string> GetCategoryDictionary();
        void Archive(Ticket ticket);
        IList<TicketCategory> AssignCategories(Ticket ticket, IEnumerable<int> categoryIds);
        string UploadAttachment(HttpPostedFileBase file);
        void AssociateAttachments(Ticket ticket, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames);
        IEnumerable<Ticket> GetTickets(int? page = null, int? pageSize = null);
    }
}
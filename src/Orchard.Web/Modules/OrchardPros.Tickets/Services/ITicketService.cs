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
        IEnumerable<TermPart> GetTags();
        Ticket Create(ExpertPart user, string title, string description, TicketType type = TicketType.Question, Action<Ticket> initialize = null);
        int CalculateExperience(ExpertPart user);
        Ticket GetTicket(int id);
        IDictionary<int, string> GetCategoryDictionary();
        IDictionary<int, string> GetTagDictionary();
        void Archive(Ticket ticket);
        void AssignCategories(Ticket ticket, IEnumerable<int> categoryIds);
        void AssignTags(Ticket ticket, string tags);
        string UploadAttachment(HttpPostedFileBase file);
        void AssociateAttachments(Ticket ticket, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames);
        IPagedList<TicketSummary> GetSummarizedTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest);
        IEnumerable<TermPart> GetTagsFor(Ticket ticket);
    }
}
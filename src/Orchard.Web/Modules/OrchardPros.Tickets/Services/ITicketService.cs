using System;
using System.Collections.Generic;
using System.Web;
using Orchard;
using Orchard.Security;
using Orchard.Taxonomies.Models;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface ITicketService : IDependency {
        IEnumerable<TicketPart> GetTicketsFor(int userId);
        IEnumerable<TermPart> GetCategories();
        IEnumerable<TermPart> GetCategoriesFor(int ticketId);
        IEnumerable<TermPart> GetTags();
        IEnumerable<TermPart> GetTagsFor(int ticketId);
        TicketPart Create(ExpertPart user, string subject, string body, TicketType type = TicketType.Question, Action<TicketPart> initialize = null);
        int CalculateExperience(ExpertPart user);
        TimeSpan GetRemainingTimeFor(TicketPart ticket);
        TicketPart GetTicket(int id);
        IDictionary<int, string> GetCategoryDictionary();
        IDictionary<int, string> GetTagDictionary();
        void AssignCategories(TicketPart ticket, IEnumerable<int> categoryIds);
        void AssignTags(TicketPart ticket, string tags);
        string UploadAttachment(HttpPostedFileBase file);
        void AssociateAttachments(TicketPart ticket, IEnumerable<string> uploadedFileNames, IEnumerable<string> originalFileNames);
        IPagedList<TicketPart> GetTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest);
        
        /// <summary>
        /// Returns the date the ticket or any of its replies was modified.
        /// </summary>
        DateTime? GetLastModifiedUtcFor(TicketPart ticket);

        /// <summary>
        /// Returns the user that last modified the ticket or any of its replies.
        /// </summary>
        IUser GetLastModifierFor(TicketPart ticket);

        void Publish(TicketPart ticket);
    }
}
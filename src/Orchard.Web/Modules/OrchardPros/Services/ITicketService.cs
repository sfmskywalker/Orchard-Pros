using System;
using System.Collections.Generic;
using Orchard;
using Orchard.Security;
using Orchard.Taxonomies.Models;
using OrchardPros.Models;

namespace OrchardPros.Services {
    public interface ITicketService : IDependency {
        IPagedList<TicketPart> GetTicketsFor(int userId, int? skip = null, int? take = null);
        IEnumerable<TermPart> GetCategories();
        IEnumerable<TermPart> GetCategoriesFor(int ticketId);
        IEnumerable<TermPart> GetTags();
        IEnumerable<TermPart> GetTagsFor(int ticketId);
        TicketPart Create(UserProfilePart user, string subject, string body, TicketType type = TicketType.Question, Action<TicketPart> initialize = null);
        int CalculateExperience(UserProfilePart user);
        TimeSpan GetRemainingTimeFor(TicketPart ticket);
        TicketPart GetTicket(int id);
        void AssignCategories(TicketPart ticket, IEnumerable<int> categoryIds);
        void AssignTags(TicketPart ticket, string tags);
        IPagedList<TicketPart> GetTickets(int? skip = null, int? take = null, TicketsCriteria criteria = TicketsCriteria.Latest, int? categoryId = null, int? tagId = null);
        
        /// <summary>
        /// Returns the date the ticket or any of its replies was modified.
        /// </summary>
        DateTime? GetLastModifiedUtcFor(TicketPart ticket);

        /// <summary>
        /// Returns the user that last modified the ticket or any of its replies.
        /// </summary>
        IUser GetLastModifierFor(TicketPart ticket);

        void Publish(TicketPart ticket);
        void Solve(TicketPart ticket, ReplyPart reply);
        IEnumerable<TicketPart> GetSolvedTicketsFor(int userId);
    }
}
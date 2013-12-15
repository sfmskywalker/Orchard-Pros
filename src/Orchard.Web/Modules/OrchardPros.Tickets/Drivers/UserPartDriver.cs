using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Users.Models;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Drivers {
    public class UserPartDriver : ContentPartDriver<UserPart> {
        private readonly ITicketService _ticketService;
        private readonly IRepository<UserPartRecord> _userRepository;
        private readonly IRepository<TitlePartRecord> _titleRepository;

        public UserPartDriver(ITicketService ticketService, IRepository<UserPartRecord> userRepository, IRepository<TitlePartRecord> titleRepository) {
            _ticketService = ticketService;
            _userRepository = userRepository;
            _titleRepository = titleRepository;
        }

        protected override DriverResult Editor(UserPart part, dynamic shapeHelper) {
            return ContentShape("Parts_User_Tickets", () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/User.Tickets",
                Model: shapeHelper.ViewModel(UserPart: part, Tickets: GetTicketsDataShape(part.Id, shapeHelper).ToArray()),
                Prefix: Prefix));
        }

        protected override DriverResult Editor(UserPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }

        private IEnumerable<dynamic> GetTicketsDataShape(int userId, dynamic shapeHelper) {
            return from ticket in _ticketService.GetTicketsFor(userId)
                from user in _userRepository.Table
                where user.Id == ticket.Id
                from categoryTitle in _titleRepository.Table
                where categoryTitle.Id == ticket.CategoryId
                select shapeHelper.Ticket(
                    Id: ticket.Id,
                    UserId: user.Id,
                    UserName: user.UserName,
                    CategoryId: categoryTitle.Id,
                    CategoryName: categoryTitle.Title,
                    Description: ticket.Description,
                    Type: ticket.Type,
                    Title: ticket.Title,
                    Tags: ticket.Tags,
                    Bounty: ticket.Bounty,
                    DeadlineUtc: ticket.DeadlineUtc,
                    ExperiencePoints: ticket.ExperiencePoints,
                    CreatedUtc: ticket.CreatedUtc,
                    LastModifiedUtc: ticket.LastModifiedUtc,
                    Solved: ticket.Solved,
                    SolvedUtc: ticket.SolvedUtc,
                    AnswerId: ticket.AnswerId);

        }
    }
}
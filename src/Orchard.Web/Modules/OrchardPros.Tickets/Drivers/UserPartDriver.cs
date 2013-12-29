using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Users.Models;
using OrchardPros.Tickets.Services;
using OrchardPros.Tickets.ViewModels;

namespace OrchardPros.Tickets.Drivers {
    public class UserPartDriver : ContentPartDriver<UserPart> {
        private readonly ITicketService _ticketService;
        private readonly IRepository<UserPartRecord> _userRepository;

        public UserPartDriver(ITicketService ticketService, IRepository<UserPartRecord> userRepository) {
            _ticketService = ticketService;
            _userRepository = userRepository;
        }

        protected override DriverResult Editor(UserPart part, dynamic shapeHelper) {
            return ContentShape("Parts_User_Tickets", () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/User.Tickets",
                Model: shapeHelper.ViewModel(UserPart: part, Tickets: GetTicketsDataShape(part.Id).ToArray()),
                Prefix: Prefix));
        }

        protected override DriverResult Editor(UserPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }

        private IEnumerable<TicketRow> GetTicketsDataShape(int userId) {
            var categoryDictionary = _ticketService.GetCategoryDictionary();
            var tagDictionary = _ticketService.GetTagDictionary();
            return from ticket in _ticketService.GetTicketsFor(userId)
                   from user in _userRepository.Table
                   where user.Id == ticket.UserId
                   select new TicketRow {
                       Id = ticket.Id,
                       UserId = user.Id,
                       UserName = user.UserName,
                       Categories = String.Join(", ", ticket.Categories.Where(x => categoryDictionary.ContainsKey(x.CategoryId)).Select(x => categoryDictionary[x.CategoryId])),
                       Description = ticket.Description,
                       Type = ticket.Type,
                       Title = ticket.Title,
                       Tags = String.Join(", ", ticket.Tags.Where(x => tagDictionary.ContainsKey(x.TagId)).Select(x => tagDictionary[x.TagId])),
                       Bounty = ticket.Bounty,
                       DeadlineUtc = ticket.DeadlineUtc,
                       ExperiencePoints = ticket.ExperiencePoints,
                       CreatedUtc = ticket.CreatedUtc,
                       LastModifiedUtc = ticket.ModifiedUtc,
                       SolvedUtc = ticket.SolvedUtc,
                       AnswerId = ticket.AnswerId
                   };
        }
    }
}
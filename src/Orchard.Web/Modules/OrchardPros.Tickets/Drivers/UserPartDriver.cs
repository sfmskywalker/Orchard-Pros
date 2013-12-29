using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Users.Models;
using OrchardPros.Tickets.Services;

namespace OrchardPros.Tickets.Drivers {
    public class UserPartDriver : ContentPartDriver<UserPart> {
        private readonly ITicketService _ticketService;

        public UserPartDriver(ITicketService ticketService) {
            _ticketService = ticketService;
        }

        protected override DriverResult Editor(UserPart part, dynamic shapeHelper) {
            return ContentShape("Parts_User_Tickets", () => shapeHelper.EditorTemplate(
                TemplateName: "Parts/User.Tickets",
                Model: shapeHelper.ViewModel(UserPart: part, Tickets: _ticketService.GetTicketsFor(part.Id).ToArray()),
                Prefix: Prefix));
        }

        protected override DriverResult Editor(UserPart part, IUpdateModel updater, dynamic shapeHelper) {
            return Editor(part, shapeHelper);
        }
    }
}
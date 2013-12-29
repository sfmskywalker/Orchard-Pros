using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Drivers {
    public class TicketPartDriver : ContentPartDriver<TicketPart> {
        protected override DriverResult Display(TicketPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Ticket", () => shapeHelper.Parts_Ticket());
        }

        protected override DriverResult Editor(TicketPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Ticket_Edit", () => shapeHelper.EditorTemplate(TemplateName: "Parts/Ticket", Models: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(TicketPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }   
    }
}
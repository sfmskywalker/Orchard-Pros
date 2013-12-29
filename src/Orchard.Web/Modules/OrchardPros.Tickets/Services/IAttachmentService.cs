using System.Collections.Generic;
using Orchard;
using OrchardPros.Tickets.Models;

namespace OrchardPros.Tickets.Services {
    public interface IAttachmentService : IDependency {
        IEnumerable<AttachmentPart> GetAttachments(IEnumerable<int> ids);
    }
}
using System.Collections.Generic;
using Orchard.ContentManagement;

namespace OrchardPros.Tickets.Models {
    public class AttachmentsHolderPart : ContentPart {
        public IList<int> AttachmentIds {
            get { return this.Retrieve(x => x.AttachmentIds); }
            set { this.Store(x => x.AttachmentIds, value); }
        }
    }
}
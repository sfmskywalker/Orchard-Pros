using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.Core.Common.Utilities;
using OrchardPros.Helpers;

namespace OrchardPros.Models {
    public class AttachmentsHolderPart : ContentPart {
        internal LazyField<IEnumerable<AttachmentPart>> AttachmentsField = new LazyField<IEnumerable<AttachmentPart>>();

        public IEnumerable<int> AttachmentIds {
            get { return Retrieve<string>("Attachments").Split<int>(); }
            set { Store("Attachments", value.Join()); }
        }

        public IEnumerable<AttachmentPart> Attachments {
            get { return AttachmentsField.Value; }
            set { AttachmentsField.Value = value; }
        }
    }
}
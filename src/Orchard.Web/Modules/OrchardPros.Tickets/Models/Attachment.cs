using Orchard.ContentManagement;

namespace OrchardPros.Tickets.Models {
    public class AttachmentPart : ContentPart {
        public string FileName {
            get { return this.Retrieve(x => x.FileName); }
            set { this.Store(x => x.FileName, value); }
        }

        public int DownloadCount {
            get { return this.Retrieve(x => x.DownloadCount); }
            set { this.Store(x => x.DownloadCount, value); }
        }
    }
}
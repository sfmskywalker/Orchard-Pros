using Orchard.ContentManagement;

namespace OrchardPros.Models {
    public class AttachmentPart : ContentPart {
        public string OriginalFileName {
            get { return this.Retrieve(x => x.OriginalFileName); }
            set { this.Store(x => x.OriginalFileName, value); }
        }

        public string LocalFileName {
            get { return this.Retrieve(x => x.LocalFileName); }
            set { this.Store(x => x.LocalFileName, value); }
        }

        public long FileSize {
            get { return this.Retrieve(x => x.FileSize); }
            set { this.Store(x => x.FileSize, value); }
        }

        public string MimeType {
            get { return this.Retrieve(x => x.MimeType); }
            set { this.Store(x => x.MimeType, value); }
        }

        public int DownloadCount {
            get { return this.Retrieve(x => x.DownloadCount); }
            set { this.Store(x => x.DownloadCount, value); }
        }
    }
}
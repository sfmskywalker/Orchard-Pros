using System.Collections.Generic;

namespace OrchardPros.Tickets.ViewModels {
    public class AttachmentsViewModel {
        public IList<string> UploadedFileNames { get; set; }
        public IList<string> OriginalFileNames { get; set; }
    }
}
using System.Collections.Generic;

namespace OrchardPros.ViewModels {
    public class AttachmentsViewModel {
        public IList<string> UploadedFileNames { get; set; }
        public IList<string> OriginalFileNames { get; set; }

        public IList<AttachmentViewModel> CurrentFiles { get; set; }
    }
}
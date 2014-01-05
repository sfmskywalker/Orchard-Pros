using System.ComponentModel.DataAnnotations;

namespace OrchardPros.ViewModels {
    public class ReplyViewModel {
        public int ContentItemId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
        public int? ParentReplyId { get; set; }
        public AttachmentsViewModel Attachments { get; set; }
    }
}
namespace OrchardPros.Tickets.ViewModels {
    public class ReplyViewModel {
        public int ContentItemId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int? ParentReplyId { get; set; }
        public AttachmentsViewModel Attachments { get; set; }
    }
}
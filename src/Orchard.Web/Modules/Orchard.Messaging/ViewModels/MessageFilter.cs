using Orchard.Messaging.Models;

namespace Orchard.Messaging.ViewModels {
    public class MessageFilter {
        public QueuedMessageStatus? Status { get; set; }
    }
}
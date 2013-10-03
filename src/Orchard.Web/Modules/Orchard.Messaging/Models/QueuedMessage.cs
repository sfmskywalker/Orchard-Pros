using System;

namespace Orchard.Messaging.Models {
    public class QueuedMessage {
        public virtual int Id { get; set; }
        public virtual int QueueId { get; set; }
        public MessagePriority Priority { get; set; }
        public virtual string ChannelName { get; set; }
        public virtual string Recipients { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Body { get; set; }
        public virtual string ShapeName { get; set; }
        public virtual string PropertyBag { get; set; }
        public virtual QueuedMessageStatus Status { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual DateTime? ExecutedUtc { get; set; }
        public virtual string ResponseMessage { get; set; }
    }
}
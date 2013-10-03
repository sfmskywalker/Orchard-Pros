namespace Orchard.Messaging.Models {
    public class MessagePriority {
        public virtual int Id { get; set; }
        public virtual int Value { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayText { get; set; }
    }
}
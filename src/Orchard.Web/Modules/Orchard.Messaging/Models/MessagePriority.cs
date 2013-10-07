using System;

namespace Orchard.Messaging.Models {
    public class MessagePriority {
        public virtual int Id { get; set; }
        public virtual int Rank { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayText { get; set; }

        public override string ToString() {
            return String.Format("{0} - {1}", Rank, Name);
        }
    }
}
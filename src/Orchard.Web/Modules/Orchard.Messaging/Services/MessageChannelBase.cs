using Orchard.Messaging.Models;

namespace Orchard.Messaging.Services {
    public abstract class MessageChannelBase : Component, IMessageChannel {
        public virtual string Name {
            get { return GetType().Name.Replace("MessageChannel", ""); }
        }

        public abstract void Send(QueuedMessage message);
        public virtual void Dispose() { }

        public override string ToString() {
            return Name;
        }
    }
}
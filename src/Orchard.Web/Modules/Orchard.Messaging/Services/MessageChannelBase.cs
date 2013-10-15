using System;
using Orchard.Messaging.Models;

namespace Orchard.Messaging.Services {
    public abstract class MessageChannelBase : Component, IMessageChannel {
        ~MessageChannelBase() {
            Dispose(false);
        }

        public virtual string Name {
            get { return GetType().Name.Replace("MessageChannel", ""); }
        }

        public abstract void Send(QueuedMessage message);

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString() {
            return Name;
        }

        protected virtual void Dispose(bool disposing) { }
    }
}
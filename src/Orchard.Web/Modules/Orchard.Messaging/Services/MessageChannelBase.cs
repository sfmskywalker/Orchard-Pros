using System;
using Orchard.Logging;
using Orchard.Messaging.Models;

namespace Orchard.Messaging.Services {
    public abstract class MessageChannelBase : Component, IMessageChannel {
        public virtual string Name {
            get { return GetType().Name; }
        }

        public virtual void Send(QueuedMessage message) {
            Logger.Debug("Sending message ID {0}", message.Id);
            try {
                OnSend(message);
                Logger.Debug("Sent message ID {0}", message.Id);
            }
            catch (Exception e) {
                message.Status = QueuedMessageStatus.Faulted;
                message.Result = e.ToString();
                Logger.Error(e, "An unexpected error while sending message {0}. Error message: {1}", message.Id, e);
            }
        }

        public override string ToString() {
            return Name;
        }

        protected abstract void OnSend(QueuedMessage message);
    }
}
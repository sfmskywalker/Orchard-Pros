using Orchard.Messaging.Models;

namespace Orchard.Messaging.Services {
    public interface IMessageChannel : IDependency {
        string Name { get; }
        void Send(QueuedMessage message);
    }
}
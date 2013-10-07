using Orchard.Environment.Extensions;
using Orchard.Tasks;

namespace Orchard.Messaging.Services {
    public interface IMessageQueueBackgroundTask : IBackgroundTask {
    }

    [OrchardFeature("Orchard.Messaging.Queuing")]
    public class MessageQueueBackgroundTask : Component, IMessageQueueBackgroundTask {
        private readonly IMessageQueueProcessor _messageQueueProcessor;
        public MessageQueueBackgroundTask(IMessageQueueProcessor messageQueueProcessor) {
            _messageQueueProcessor = messageQueueProcessor;
        }

        public void Sweep() {
            _messageQueueProcessor.ProcessQueues();
        }
    }
}
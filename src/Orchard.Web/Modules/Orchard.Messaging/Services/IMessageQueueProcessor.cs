using System;
using System.Linq;
using Orchard.Messaging.Models;
using Orchard.Services;

namespace Orchard.Messaging.Services {
    public interface IMessageQueueProcessor : IDependency {
        void ProcessQueues();
    }

    public class MessageQueueProcessor : IMessageQueueProcessor {
        private readonly IMessageQueueManager _manager;
        private readonly IClock _clock;

        public MessageQueueProcessor(IMessageQueueManager manager, IClock clock) {
            _manager = manager;
            _clock = clock;
        }

        public void ProcessQueues() {
            var queues = _manager.GetIdleQueues().ToList();
            var queuesToProcess = from queue in queues 
                                  let lastProcessedUtc = queue.EndedUtc.GetValueOrDefault()
                                  let timeSinceLastProcessAction = _clock.UtcNow - lastProcessedUtc
                                  where timeSinceLastProcessAction > queue.UpdateFrequency
                                  select queue;

            foreach (var queue in queuesToProcess.AsParallel()) {
                var messages = _manager.EnterProcessingStatus(queue);

                foreach (var message in messages.AsParallel()) {
                    ProcessMessage(message);
                    if (!queue.HasAvailableTime)
                        break;
                }

                _manager.ExitProcessingStatus(queue);
            }
        }

        private void ProcessMessage(QueuedMessage message) {
            var channel = message.Channel;
            channel.Send(message);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Orchard.Messaging.Models;
using Orchard.Services;

namespace Orchard.Messaging.Services {
    public interface IMessageQueueProcessor : IDependency {
        void ProcessQueues();
    }

    public class MessageQueueProcessor : Component, IMessageQueueProcessor {
        private readonly IMessageQueueManager _manager;
        private readonly IClock _clock;

        public MessageQueueProcessor(IMessageQueueManager manager, IClock clock) {
            _manager = manager;
            _clock = clock;
        }

        public void ProcessQueues() {
            var queuesToProcess = GetQueuesToProcess();

            foreach (var queue in queuesToProcess.AsParallel()) {
                ProcessQueue(queue);
            }
        }

        private IEnumerable<MessageQueue> GetQueuesToProcess() {
            var queues = _manager.GetIdleQueues().ToList();
            var q = from queue in queues
                    where queue.Status == MessageQueueStatus.Idle
                    let lastProcessedUtc = queue.EndedUtc.GetValueOrDefault()
                    let timeSinceLastProcessAction = _clock.UtcNow - lastProcessedUtc
                    where timeSinceLastProcessAction > queue.UpdateFrequency
                    select queue;
            return q;
        }

        private void ProcessQueue(MessageQueue queue) {
            var messages = _manager.EnterProcessingStatus(queue);

            foreach (var message in messages.AsParallel()) {
                ProcessMessage(message);
                if (!queue.HasAvailableTime)
                    break;
            }

            _manager.ExitProcessingStatus(queue);
        }

        private void ProcessMessage(QueuedMessage message) {
            var channel = message.Channel;

            message.StartedUtc = _clock.UtcNow;
            message.Status = QueuedMessageStatus.Sending;

            Logger.Debug("Sending message ID {0}.", message.Id);
            if (!message.Recipients.Any()) {
                message.Status = QueuedMessageStatus.Faulted;
                message.Result = String.Format("Cannot send message {0} because at least on recipient is required.", message.Id);
                Logger.Error(message.Result);
                return;
            }
            try {
                channel.Send(message);
                message.Status = QueuedMessageStatus.Sent;
                Logger.Debug("Sent message ID {0}.", message.Id);
            }
            catch (Exception e) {
                message.Status = QueuedMessageStatus.Faulted;
                message.Result = e.ToString();
                Logger.Error(e, "An unexpected error while sending message {0}. Error message: {1}.", message.Id, e);
            }
            finally {
                message.CompletedUtc = _clock.UtcNow;
            }
        }
    }
}
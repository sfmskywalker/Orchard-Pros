using System;
using System.Collections.Generic;
using Orchard.Messaging.Models;
using Orchard.Messaging.Services;

namespace Orchard.Messaging.Tests {
    public class StubMessageQueueManager : IMessageQueueManager {
        public QueuedMessage Send(MessageRecipient recipient, string channelName, string subject = null, string body = null, string shapeName = null, object propertyBag = null, MessagePriority priority = null, int? queueId = null) {
            throw new NotImplementedException();
        }

        public QueuedMessage Send(IEnumerable<MessageRecipient> recipients, string channelName, string subject = null, string body = null, string shapeName = null, object propertyBag = null, MessagePriority priority = null, int? queueId = null) {
            throw new NotImplementedException();
        }

        public QueuedMessage Send(string recipient, string channelName, string subject = null, string body = null, string shapeName = null, object propertyBag = null, MessagePriority priority = null, int? queueId = null) {
            throw new NotImplementedException();
        }

        public QueuedMessage Send(IEnumerable<string> recipients, string channelName, string subject = null, string body = null, string shapeName = null, object propertyBag = null, MessagePriority priority = null, int? queueId = null) {
            throw new NotImplementedException();
        }

        public MessageQueue GetQueue(int id) {
            throw new NotImplementedException();
        }

        public MessageQueue GetDefaultQueue() {
            throw new NotImplementedException();
        }

        public MessagePriority GetPriority(int id) {
            throw new NotImplementedException();
        }

        public MessagePriority GetPriority(string name) {
            throw new NotImplementedException();
        }

        public MessagePriority GetDefaultPriority() {
            throw new NotImplementedException();
        }

        public IEnumerable<MessagePriority> CreateDefaultPrioritySet() {
            throw new NotImplementedException();
        }

        public IEnumerable<MessageQueue> GetIdleQueues() {
            throw new NotImplementedException();
        }

        public IEnumerable<QueuedMessage> EnterProcessingStatus(MessageQueue queue) {
            throw new NotImplementedException();
        }

        public void ExitProcessingStatus(MessageQueue queue) {
            throw new NotImplementedException();
        }
    }
}
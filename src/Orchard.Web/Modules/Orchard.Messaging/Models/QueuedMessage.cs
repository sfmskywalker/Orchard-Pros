using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Messaging.Services;

namespace Orchard.Messaging.Models {
    public class QueuedMessage {
        // ReSharper disable InconsistentNaming
        public Lazy<MessageQueue> QueueField;
        public Lazy<IMessageChannel> ChannelField;
        public Lazy<IEnumerable<MessageRecipient>> RecipientsField;
        // ReSharper restore InconsistentNaming

        public QueuedMessage(QueuedMessageRecord record) {
            Record = record;
        }

        public QueuedMessageRecord Record {
            get; private set;
        }

        public int Id {
            get { return Record.Id; }
        }
        
        public MessagePriority Priority {
            get { return Record.Priority; }
            set { Record.Priority = value; }
        }

        public string Subject {
            get { return Record.Subject; }
            set { Record.Subject = value; }
        }

        public string Body {
            get { return Record.Body; }
            set { Record.Body = value; }
        }

        public QueuedMessageStatus Status {
            get { return Record.Status; }
            internal set { Record.Status = value; }
        }

        public string Result {
            get { return Record.Result; }
            set { Record.Result = value; }
        }

        public DateTime CreatedUtc {
            get { return Record.CreatedUtc; }
        }

        public DateTime? StartedUtc {
            get { return Record.StartedUtc; }
            internal set { Record.StartedUtc = value; }
        }

        public DateTime? CompletedUtc {
            get { return Record.CompletedUtc; }
            internal set { Record.CompletedUtc = value; }
        }

        public MessageQueue Queue {
            get { return QueueField.Value; }
        }

        public IMessageChannel Channel {
            get { return ChannelField.Value; }
        }

        public IEnumerable<MessageRecipient> Recipients {
            get { return RecipientsField.Value; }
        }

        public override string ToString() {
            return String.Format("Subject: {0}, Recipients: {1}", Subject, String.Join(", ", Recipients.Select(x => x.ToString())));
        }
    }
}
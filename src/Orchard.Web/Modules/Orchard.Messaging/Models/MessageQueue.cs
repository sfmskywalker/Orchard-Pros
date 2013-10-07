using System;

namespace Orchard.Messaging.Models {
    public class MessageQueue {
        // ReSharper disable InconsistentNaming
        internal Func<TimeSpan> AvailableTimeFunc;
        internal Func<bool> HasAvailableTimeFunc;
        // ReSharper restore InconsistentNaming

        internal MessageQueue(MessageQueueRecord record) {
            Record = record;
        }

        internal MessageQueueRecord Record { get; private set; }

        public int Id {
            get { return Record.Id; }
        }

        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public MessageQueueStatus Status {
            get { return Record.Status; }
            internal set { Record.Status = value; }
        }

        public DateTime? StartedUtc {
            get { return Record.StartedUtc; }
            internal set { Record.StartedUtc = value; }
        }

        public DateTime? EndedUtc {
            get { return Record.EndedUtc; }
            internal set { Record.EndedUtc = value; }
        }

        /// <summary>
        /// Update frequency.
        /// </summary>
        public TimeSpan UpdateFrequency {
            get { return TimeSpan.FromSeconds(Record.UpdateFrequency); }
            set { Record.UpdateFrequency = (int) value.TotalSeconds; }
        }

        /// <summary>
        /// The time this queue is given to process messages
        /// </summary>
        public TimeSpan TimeSlice {
            get { return TimeSpan.FromSeconds(Record.TimeSlice); }
            set { Record.TimeSlice = (int)value.TotalSeconds; }
        }

        public TimeSpan AvailableTime {
            get { return AvailableTimeFunc(); }
        }

        public bool HasAvailableTime {
            get { return HasAvailableTimeFunc(); }
        }

        public override string ToString() {
            return String.Format("{0} - {1}", Name, Status);
        }
    }
}
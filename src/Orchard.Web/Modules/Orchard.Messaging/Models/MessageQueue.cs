namespace Orchard.Messaging.Models {
    public class MessageQueue {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public MessageQueueStatus Status { get; set; }

        /// <summary>
        /// Update Interval in seconds.
        /// </summary>
        public int UpdateInterval { get; set; }

        /// <summary>
        /// The number of seconds this queue is allowed to process messages
        /// </summary>
        public int TimeSlice { get; set; }
    }
}
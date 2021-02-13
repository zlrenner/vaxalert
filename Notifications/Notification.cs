using System;

namespace vaxalert.Stores
{
    public class Notification
    {
        public Notification(
            Guid id,
            DateTime time,
            Subscriber subscriber,
            string reason)
        {
            Id = id;
            Time = time;
            Subscriber = subscriber;
            Reason = reason;
        }
        
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public Subscriber Subscriber { get; set; }
        public string Reason { get; set; }
    }
}
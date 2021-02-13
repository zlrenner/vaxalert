using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace vaxalert.Stores
{
    public class NotificationStore : INotificationStore
    {
        private readonly List<Notification> _notifications = new List<Notification>();

        public Task LogNotificationAsync(Subscriber subscriber, string reason)
        {
            _notifications.Add(new Notification(Guid.NewGuid(), DateTime.UtcNow, subscriber, reason));
            return Task.CompletedTask;
        }

        public Task<IList<Notification>> GetNotificationsAsync()
        {
            return Task.FromResult((IList<Notification>) _notifications);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vaxalert.Stores;

namespace vaxalert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly ILogger<NotificationsController> _logger;
        private readonly INotificationStore _notificationStore;

        public NotificationsController(
            ILogger<NotificationsController> logger,
            INotificationStore notificationStore)
        {
            _logger = logger;
            _notificationStore = notificationStore;
        }


        [HttpGet]
        public async Task<IEnumerable<Notification>> Get()
        {
            var notifications = await _notificationStore.GetNotificationsAsync();
            foreach (var notification in notifications)
            {
                if (notification?.Subscriber?.Phone != null)
                {
                    notification.Subscriber.Phone = notification.Subscriber.Phone.Substring(0, 6) + "...";
                }

                if (notification?.Subscriber?.Email != null)
                {
                    notification.Subscriber.Email = notification.Subscriber.Email.Substring(0, 3) + "..." +
                                                    notification.Subscriber.Email.Substring(
                                                        notification.Subscriber.Email.Length - 4);
                }
            }

            return notifications;
        }
    }
}

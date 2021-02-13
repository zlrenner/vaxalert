using System.Collections.Generic;
using System.Threading.Tasks;

namespace vaxalert.Stores
{
    public interface INotificationStore
    {
        Task LogNotificationAsync(Subscriber subscriber, string reason);
        Task<IList<Notification>> GetNotificationsAsync();
    }
}
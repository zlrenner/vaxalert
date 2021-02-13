using System.Collections.Generic;
using System.Threading.Tasks;

namespace vaxalert.Stores
{
    public interface ISubscriptionStore
    {
        Task AddSubscriptionAsync(string eventKey, Subscriber subscriber);
        Task<IEnumerable<Subscriber>> GetSubscribersAsync(string eventKey);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync();
    }
}
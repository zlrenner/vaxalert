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
    public class SubscriptionsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly ISubscriptionStore _subscriptionStore;

        public SubscriptionsController(
            ILogger<AppointmentsController> logger,
            ISubscriptionStore subscriptionStore)
        {
            _logger = logger;
            _subscriptionStore = subscriptionStore;
        }
        
        [HttpGet]
        public async Task Add()
        {
            await _subscriptionStore.AddSubscriptionAsync("SeattleU Volunteering", new Subscriber { Email = "zlrenner@outlook.com" });
            await _subscriptionStore.AddSubscriptionAsync("SeattleU Volunteering - Slots", new Subscriber { Email = "zlrenner@outlook.com" });
        }
    }
}

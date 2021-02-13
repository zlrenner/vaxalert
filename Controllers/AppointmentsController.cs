using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vaxalert.Stores;

namespace vaxalert.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IAppointmentStore _appointmentStore;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IAppointmentStore appointmentStore)
        {
            _logger = logger;
            _appointmentStore = appointmentStore;
        }

        [HttpGet]
        public async Task<IEnumerable<Appointment>> Get()
        {
            return await _appointmentStore.GetAppointmentsAsync();
        }
    }
}

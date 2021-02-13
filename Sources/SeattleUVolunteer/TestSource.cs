using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PlaywrightSharp;
using vaxalert.Controllers;

namespace vaxalert.Providers
{
    public class TestSource: IAppointmentSource
    {
        
        public string Name { get; } = "Test";

        public async Task<IList<Appointment>> GetAppointmentsAsync()
        {
            var appointments = new List<Appointment>();
            appointments.Add(new Appointment(
                Name,
                "test", 
                "test",
                DateTime.UtcNow,
                null));

            return appointments;
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using vaxalert.Controllers;

namespace vaxalert.Providers
{
    public interface IAppointmentSource
    {
        string Name { get; }

        Task<IList<Appointment>> GetAppointmentsAsync();
    }
}
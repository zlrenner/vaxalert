using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using vaxalert.Controllers;

namespace vaxalert.Stores
{
    public interface IAppointmentStore
    {
        public Task<IList<Appointment>> AddAppointmentsAsync(SourceScan scan, IList<Appointment> appointments);
        public Task<IList<Appointment>> GetAppointmentsAsync();
    }
}
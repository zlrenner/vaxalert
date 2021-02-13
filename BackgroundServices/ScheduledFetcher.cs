using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using vaxalert.Providers;
using vaxalert.Stores;

namespace vaxalert.BackgroundServices
{
    public class ScheduledFetcher : BackgroundService
    {
        private readonly IEnumerable<IAppointmentSource> _appointmentSources;
        private readonly IAppointmentStore _appointmentStore;
        private readonly INotificationProcessor _notificationProcessor;

        public ScheduledFetcher(
            IEnumerable<IAppointmentSource> appointmentSources,
            IAppointmentStore appointmentStore,
            INotificationProcessor notificationProcessor)
        {
            _appointmentSources = appointmentSources;
            _appointmentStore = appointmentStore;
            _notificationProcessor = notificationProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Guid scanId = Guid.NewGuid();
                DateTime scanTime = DateTime.UtcNow;

                foreach (var source in _appointmentSources)
                {
                    SourceScan scan = new SourceScan
                    {
                        Source = source.Name,
                        Id = scanId,
                        Time = scanTime
                    };

                    try
                    {
                        Console.WriteLine($"Getting appointments for {source.Name}");
                        var appointments = await source.GetAppointmentsAsync();
                        Console.WriteLine($"Adding {appointments.Count} appointments for {source.Name}");
                        var newAppointments = await _appointmentStore.AddAppointmentsAsync(scan, appointments);
                        Console.WriteLine($"Processing notifications for {source.Name}");
                        await _notificationProcessor.GenerateNotificationsAsync(newAppointments);
                        Console.WriteLine($"Done with {source.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error getting appointments for {source.Name}:\n{ex}");
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}
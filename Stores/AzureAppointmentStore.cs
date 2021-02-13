using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vaxalert.Controllers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Options;

namespace vaxalert.Stores
{
    public class AzureAppointmentStore : IAppointmentStore
    {
        private readonly Lazy<CosmosClient> _cosmosClient;
        
        private readonly IOptions<CosmosOptions> _options;

        public AzureAppointmentStore(IOptions<CosmosOptions> options)
        {
            _cosmosClient = new Lazy<CosmosClient>(GetCosmosClient);
            _options = options;
        }
        
        private CosmosClient GetCosmosClient() => new CosmosClient(_options.Value.ConnectionString);

        private async Task<Container> GetContainerAsync(string container, string partitionKey)
        {
            var client = GetCosmosClient();
            var database = client.GetDatabase("VaxAlert");
            await database.CreateContainerIfNotExistsAsync(container, partitionKey);
            return database.GetContainer(container);
        }

        private Task<Container> GetAppointmentsContainerAsync() => GetContainerAsync("Appointments", "/Source");
        // private Task<Container> GetScansContainerAsync() => GetContainerAsync("Scans", "/Source");

        public async Task<IList<Appointment>> AddAppointmentsAsync(
            SourceScan scan,
            IList<Appointment> appointments)
        {
            var container = await GetAppointmentsContainerAsync();

            if (appointments.Any())
            {
                // Get existing appointments
                var allOldAppointments = await GetAppointmentsAsync();
                var allOldSourceAppointments = allOldAppointments.Where(x => x.Source == scan.Source);
                List<Appointment> prevScanAppointments = new List<Appointment>();
                if (allOldSourceAppointments.Any())
                {
                    DateTime prevScan = allOldSourceAppointments.Max(x => x.ScanTime);
                    prevScanAppointments = allOldSourceAppointments.Where(x => x.ScanTime == prevScan).ToList();
                }

                // Process and add appointments
                foreach (var appointment in appointments)
                {
                    appointment.Id = Guid.NewGuid().ToString();
                    appointment.ScanId = scan.Id;
                    appointment.ScanTime = scan.Time;
                    await container.CreateItemAsync(appointment);
                }

                // Look for changes between the previous scan and this scan
                Console.WriteLine($"Looking for '{scan.Source}' changes between previous scan and new scan");
                Console.WriteLine($"Previous scan had {prevScanAppointments.Count} records");
                Console.WriteLine($"This new scan has {appointments.Count} records");
                var newAppointments = appointments.Except(prevScanAppointments).ToList();
                var goneAppointments = prevScanAppointments.Except(appointments).ToList();
                Console.WriteLine($"New: {newAppointments.Count}");
                Console.WriteLine($"Gone: {goneAppointments.Count}");
                return newAppointments;
            }
            else
            {
                // Add a fake appointment as a marker of the scan
                await container.CreateItemAsync(new Appointment(scan.Source, "EMPTY", "EMPTY")
                {
                    ScanId = scan.Id,
                    ScanTime = scan.Time,
                    Id = Guid.NewGuid().ToString()
                });
                return new List<Appointment>();
            }
        }

        public async Task<IList<Appointment>> GetAppointmentsAsync()
        {
            // Get the log of every appointment ever seen in the lookback period
            var appointmentHistory = await GetAppointmentHistoryAsync();
            List<string> sources = appointmentHistory.Select(x => x.Source).Distinct().ToList();
            Console.WriteLine($"Log has sources: {string.Join(", ", sources)}");

            var appointments = new List<Appointment>();
            foreach (var source in sources)
            {
                appointments.AddRange(GetAppointmentsForSource(source, appointmentHistory));
            }

            Console.WriteLine($"Returning {appointments.Count} records for all sources");
            return appointments;
        }
        
        private IList<Appointment> GetAppointmentsForSource(
            string source,
            IList<Appointment> allAppointmentHistory)
        {
            Console.WriteLine($"Deduping history for {source}");
            var sourceAppointments = allAppointmentHistory.Where(x => x.Source == source);
            DateTime latestScanForSource = sourceAppointments.Max(x => x.ScanTime);

            var dedupedAppointments = new List<Appointment>();
            var seenAppointments = new HashSet<Appointment>();
            foreach (var appointment in sourceAppointments)
            {
                if (!seenAppointments.Contains(appointment))
                {
                    // Only available if in the latest scan, otherwise historical
                    appointment.Available = appointment.ScanTime == latestScanForSource;

                    seenAppointments.Add(appointment);
                    dedupedAppointments.Add(appointment);
                }
            }

            // Empty scans have a placeholder
            var actualAppointments = dedupedAppointments.Where(x => x.Title != "EMPTY").ToList();
            Console.WriteLine($"Returning {actualAppointments.Count} records for {source}");
            return actualAppointments;
        }

        private async Task<IList<Appointment>> GetAppointmentHistoryAsync()
        {
            var lookbackTime = DateTime.UtcNow - TimeSpan.FromHours(6);
            
            var container = await GetAppointmentsContainerAsync();
            var appointments = new List<Appointment>();
            var items = container.GetItemQueryIterator<Appointment>(new QueryDefinition($"SELECT * FROM a WHERE a.ScanTime >= '{lookbackTime.ToString("o")}' ORDER BY a.ScanTime DESC"));
            while (items.HasMoreResults)
            {
                appointments.AddRange(await items.ReadNextAsync());
            }
            
            return appointments;
        }
    }
}
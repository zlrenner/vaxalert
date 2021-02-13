using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PlaywrightSharp;
using vaxalert.Controllers;

namespace vaxalert.Providers
{
    public class MassWASource: IAppointmentSource
    {
        private readonly IBrowserProvider _browserProvider;

        public MassWASource(IBrowserProvider browserProvider)
        {
            _browserProvider = browserProvider;
        }

        public string Name { get; } = "WA Mass Vaccinations";

        public async Task<IList<Appointment>> GetAppointmentsAsync()
        {
            await using var browser = await _browserProvider.GetBrowserAsync();

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync("https://prepmod.doh.wa.gov/clinic/search");
            var vaxEvents = await page.QuerySelectorAllAsync(".main-container .mt-24 .flex");

            var appointments = new List<Appointment>();
            if (vaxEvents?.Any() ?? false)
            {
                foreach (var vaxEvent in vaxEvents)
                {
                    var titleElement = await vaxEvent.QuerySelectorAsync(".title");
                    if (titleElement == null)
                    {
                        Console.WriteLine("Failed to get title for mass vax event");
                        continue;
                    }
                    var title = await titleElement.GetInnerTextAsync();
                    Console.WriteLine($"{title}");
                    var match = Regex.Match(title, "(.*) - COVID Vaccine Clinic on (.*)");
                    if (match.Success)
                    {
                        string location = match.Groups[1].Value;
                        string dateString = match.Groups[2].Value;
                        DateTime.TryParse(dateString, out DateTime date);

                        appointments.Add(new Appointment(
                            Name,
                            location,
                            "Vaccine Clinic",
                            date != default ? (DateTime?) date : null,
                            null));
                    }
                }
            }

            return appointments;
        }
    }
}
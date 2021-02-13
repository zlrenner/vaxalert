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
    public class SeattleUVolunteerSource: IAppointmentSource
    {
        private readonly IBrowserProvider _browserProvider;
        private readonly SeattleUVolunteerOptions _options;

        public SeattleUVolunteerSource(
            IBrowserProvider browserProvider,
            IOptions<SeattleUVolunteerOptions> options)
        {
            _browserProvider = browserProvider;
            _options = options.Value;
        }

        public string Name { get; } = "SeattleU Volunteering - Weeks";

        public async Task<IList<Appointment>> GetAppointmentsAsync()
        {
            await using var browser = await _browserProvider.GetBrowserAsync();
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await page.GoToAsync("https://volunteer.covidvaccineseattle.org/VolunteerRecall.aspx");
            await page.FillAsync("#ContentPlaceHolder1_TextBox1", _options.Username);
            await page.FillAsync("#ContentPlaceHolder1_TextBoxPassword", _options.Password);
            await page.ClickAsync("#ContentPlaceHolder1_Button1");
            await page.WaitForSelectorAsync("#ContentPlaceHolder1_GridView1");
            var dates = await page.QuerySelectorAllAsync("#ContentPlaceHolder1_GridView1 tbody tr:not(.SeattleGreen) td:nth-child(2)");

            var appointments = new List<Appointment>();
            if (dates?.Any() ?? false)
            {
                foreach (var date in dates)
                {
                    var title = await date.GetInnerTextAsync();
                    appointments.Add(new Appointment(
                        Name,
                        "SeattleU", 
                        title,
                        null,
                        null,
                        notes: "Only indicates registration has opened for this week. Spots may not be available."));
                }
            }

            return appointments;
        }
    }
}
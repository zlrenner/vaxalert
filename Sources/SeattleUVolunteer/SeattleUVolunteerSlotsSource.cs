using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using vaxalert.Controllers;
using TimeZone = CoreTweet.TimeZone;

namespace vaxalert.Providers
{
    public class SeattleUVolunteerSlotsSource: IAppointmentSource
    {
        private readonly IBrowserProvider _browserProvider;
        private readonly SeattleUVolunteerOptions _options;

        public SeattleUVolunteerSlotsSource(
            IBrowserProvider browserProvider,
            IOptions<SeattleUVolunteerOptions> options)
        {
            _browserProvider = browserProvider;
            _options = options.Value;
        }

        public string Name { get; } = "SeattleU Volunteering - Slots";

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
            int weekCount = (await page.QuerySelectorAllAsync("#ContentPlaceHolder1_GridView1 tbody tr:not(.SeattleGreen) td:nth-child(1) input")).Count();

            var appointments = new List<Appointment>();
            for (int i = 0; i < weekCount; i++)
            {
                await page.GoToAsync("https://volunteer.covidvaccineseattle.org/VolunteerDashboard.aspx");
                var selector = (await page.QuerySelectorAllAsync("#ContentPlaceHolder1_GridView1 tbody tr:not(.SeattleGreen) td:nth-child(1) input")).Skip(i).First();
                await selector.ClickAsync();
                await page.WaitForSelectorAsync("#ContentPlaceHolder1_UpdatePanelSchedule");
                
                Console.WriteLine($"Checking if any assignments open for week index {i}");
                var dayElements = await page.QuerySelectorAllAsync("[id^=ContentPlaceHolder1_RepeaterSchedule_UpdatePanelAssignment_]");
                Console.WriteLine($"{dayElements.Count()} days to check");
                foreach (var dayElement in dayElements)
                {
                    var parent = (await dayElement.QuerySelectorAllAsync("../..")).First();
                    bool closed = (await dayElement.QuerySelectorAllAsync("[id^=ContentPlaceHolder1_RepeaterSchedule_LabelRegistrationClosed_]")).Any();
                    if (!closed)
                    {
                        var day = await (await parent.QuerySelectorAsync("td:nth-child(1)")).GetInnerTextAsync();
                        Console.WriteLine($"Day {day} might have openings");
                        var optionsElements = await parent.QuerySelectorAllAsync("select option");
                        var roles = await Task.WhenAll(optionsElements.Select(async x => await x.GetTextContentAsync()));
                        foreach (var role in roles)
                        {
                            if (role != "Not attending this day")
                            {
                                var date = DateTime.Parse(day);
                                DateTime? time = null;
                                var timeMatch = Regex.Match(role, ".* @ (.*?[AaPp][Mm])");
                                if (timeMatch.Success)
                                {
                                    bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now); // Assume we're on a US server at least...

                                    string timeString = timeMatch.Groups[1].Value; // e.g. 6:00AM
                                    string dateTimeString = $"{timeString} {day} 2021 -{(isDst ? "7" : "8")}:00"; // e.g. 6:00AM Mon Feb 8 2021 -8:00
                                    if (DateTime.TryParse(dateTimeString, out DateTime parsedTime))
                                    {

                                        time = parsedTime.ToUniversalTime();
                                        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
                                        if (currentTime >= time)
                                        {
                                            Console.WriteLine($"Ignoring {role} at {time} since it's before the current time {currentTime}");
                                            continue;
                                        }
                                    }
                                }

                                appointments.Add(new Appointment(Name, "SeattleU", $"{role} on {day}", date: date, time: time));
                            }
                        }
                    }
                }
            }

            return appointments;
        }
    }
}
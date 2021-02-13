using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneNumbers;
using vaxalert.Controllers;
using vaxalert.Stores.SMS;

namespace vaxalert.Stores
{
    public interface INotificationProcessor
    {
        Task GenerateNotificationsAsync(IEnumerable<Appointment> newAppointments);
    }

    public class NotificationProcessor : INotificationProcessor
    {
        private readonly ISubscriptionStore _subscriptionStore;
        private readonly INotificationStore _notificationStore;
        private readonly ISmsProvider _smsProvider;

        public NotificationProcessor(
            ISubscriptionStore subscriptionStore,
            INotificationStore notificationStore,
            ISmsProvider smsProvider)
        {
            _subscriptionStore = subscriptionStore;
            _notificationStore = notificationStore;
            _smsProvider = smsProvider;
        }

        public async Task GenerateNotificationsAsync(IEnumerable<Appointment> newAppointments)
        {
            var emailsToNotify = new Dictionary<string, List<Appointment>>();
            var phonesToNotify = new Dictionary<string, List<Appointment>>();

            foreach (var appointment in newAppointments)
            {
                Console.WriteLine($"Processing notifications for new appointment {appointment.Id}");

                string eventKey = $"{appointment.Source}";
                var subscribers = await _subscriptionStore.GetSubscribersAsync(eventKey);
                
                foreach (var subscriber in subscribers)
                {
                    Collect(subscriber.Email, emailsToNotify, appointment);
                    Collect(subscriber.Phone, phonesToNotify, appointment);
                }
            }
            
            // Send emails
            foreach (var email in emailsToNotify)
            {
                string reason = string.Join(" ", email.Value.GroupBy(x => x.Source).Select(x => $"Found {x.Count()} appointment{(x.Count() != 1 ? "s" : "")} for {x.Key}."));
                await _notificationStore.LogNotificationAsync(new Subscriber { Email = email.Key }, reason);
            }

            foreach (var phoneToNotify in phonesToNotify)
            {
                // var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                // var number = phoneNumberUtil.Parse(phoneToNotify.Key, "US");
                // if (!phoneNumberUtil.IsValidNumberForRegion(number, "US"))
                // {
                //     Console.WriteLine($"Number {number} is not a valid phone number");
                //     continue;
                // }

                var sb = new StringBuilder();
                sb.Append("VaxAlert found possible appointments!\n\n");
                foreach (var appointment in phoneToNotify.Value)
                {
                    sb.Append($"{appointment.Source}: {appointment.Title}\n");
                }

                sb.Append("\nLink: https://volunteer.covidvaccineseattle.org/VolunteerDashboard.aspx\n");

                sb.Append("\nText STOP to unsubscribe");

                Console.WriteLine($"Sending SMS to {phoneToNotify.Key}");
                await _smsProvider.SendSmsAsync(phoneToNotify.Key, sb.ToString());
                string reason = string.Join(" ", phoneToNotify.Value.GroupBy(x => x.Source).Select(x => $"Found {x.Count()} appointment{(x.Count() != 1 ? "s" : "")} for {x.Key}."));
                await _notificationStore.LogNotificationAsync(new Subscriber { Phone = phoneToNotify.Key }, reason);
            }
        }

        private static void Collect(string contactInfo, Dictionary<string, List<Appointment>> contactList, Appointment appointment)
        {
            if (contactInfo != null)
            {
                if (!contactList.ContainsKey(contactInfo))
                {
                    contactList[contactInfo] = new List<Appointment>();
                }

                contactList[contactInfo].Add( appointment);
            }
        }
    }
}

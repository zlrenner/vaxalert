using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PhoneNumbers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace vaxalert.Stores.SMS
{
    public class TwilioSmsProvider : ISmsProvider
    {
        private readonly IOptions<TwilioOptions> _options;

        public TwilioSmsProvider(IOptions<TwilioOptions> options)
        {
            _options = options;
        }

        public async Task SendSmsAsync(string destination, string message)
        {
            string accountSid = _options.Value.AccountSid;
            string authToken = _options.Value.AuthToken;
            Console.WriteLine($"Using twilio account sid {accountSid}");

            TwilioClient.Init(accountSid, authToken);

            try
            {
                await MessageResource.CreateAsync(
                    body: message,
                    @from: new Twilio.Types.PhoneNumber(_options.Value.SourceNumber),
                    to: new Twilio.Types.PhoneNumber(destination)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send SMS: {ex}");
            }
        }
    }
}
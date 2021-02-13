using System.Threading.Tasks;
using PhoneNumbers;

namespace vaxalert.Stores.SMS
{
    public interface ISmsProvider
    {
        public Task SendSmsAsync(string destination, string message);
    }
}
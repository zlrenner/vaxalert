using System.Threading.Tasks;
using CoreTweet;

namespace vaxalert.Stores.Twitter
{
    public interface IMessageProvider
    {
        public Task SendMessageAsync(string message);
    }

    // public class TwitterMessageProvider : IMessageProvider
    // {
    //     public Task SendMessageAsync(string message)
    //     {
    //
    //         var tokens = OAuth.GetTokens(session, "PINCODE");
    //     }
    // }
}
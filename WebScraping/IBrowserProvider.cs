using System.Threading.Tasks;
using PlaywrightSharp;

namespace vaxalert.Providers
{
    public interface IBrowserProvider
    {
        Task<IBrowser> GetBrowserAsync();
    }
}
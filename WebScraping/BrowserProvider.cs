using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PlaywrightSharp;

namespace vaxalert.Providers
{
    public class BrowserProvider : IBrowserProvider
    {
        private AsyncLazy<IPlaywright> _playwright = new AsyncLazy<IPlaywright>(CreatePlaywrightAsync);

        private static async Task<IPlaywright> CreatePlaywrightAsync()
        {
            Console.WriteLine("Creating playwright");
            return await Playwright.CreateAsync();
        }

        public async Task<IBrowser> GetBrowserAsync()
        {
            IPlaywright playwright = await _playwright.Value;
            Console.WriteLine("Launching chromium...");
            var browser = await playwright.Chromium.LaunchAsync(executablePath: RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "browsers/chromium-844399/chrome-linux/chrome" : null);
            Console.WriteLine("Launched");
            return browser;
        }
    }
}
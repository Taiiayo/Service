using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Service.WindowsService;

namespace Service.Extensions.HostExtensions
{
    public static class WindowsHostExtensions
    {

        public static async Task RunService(this IHostBuilder hostBuilder)
        {
            if (!Environment.UserInteractive)
            {
                await hostBuilder.RunAsServiceAsync();
            }
            else
            {
                await hostBuilder.RunConsoleAsync();
            }
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Service.Extensions.HostExtensions
{
    public static class HostExtensions
    {
        public static Task RunService(this IHostBuilder hostBuilder)
        {
            return hostBuilder.RunConsoleAsync();
        }
    }
}

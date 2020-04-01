using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Extensions.HostExtensions;
using Service.Models;
using Service.Services;
using Service.Services.TaskQueue;
using Service.Worker;

namespace Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(confBuilder =>
                {
                    confBuilder.AddJsonFile("config.json");
                    confBuilder.AddCommandLine(args);
                })
                .ConfigureLogging((confLogging) =>
                {
                    confLogging.AddConsole();
                    confLogging.AddDebug();
                })
                .ConfigureServices((services) =>
                {
                    services.AddHostedService<TaskSchedulerService>();
                    services.AddHostedService<WorkerService>();
                    services.AddSingleton<Settings>();
                    services.AddSingleton<TaskHandler>();
                    services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                });

            await builder.RunService();
        }
    }
}

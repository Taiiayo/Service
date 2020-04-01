using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Models;
using Service.Services.TaskQueue;
using Service.Worker;

namespace Service.Services
{
    public class TaskSchedulerService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly IServiceProvider services;
        private readonly Settings settings;
        private readonly ILogger logger;
        private readonly Random random = new Random();
        private readonly object syncRoot = new object();

        public TaskSchedulerService(IServiceProvider services )
        {
            this.services = services;
            this.settings = services.GetRequiredService<Settings>();
            this.logger = services.GetRequiredService<ILogger<TaskSchedulerService>>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var interval = settings?.RunInterval ?? 0;

            if(interval == 0)
            {
                logger.LogWarning("checkInterval is not defined in settings. Set to 60");
                interval = 60;
            }

            timer = new Timer(
                (e) => ProcessTask(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(interval));
            return Task.CompletedTask;
        }

        private void ProcessTask()
        {
            if (Monitor.TryEnter(syncRoot))
            {
                logger.LogInformation($"Process task started!");
                for (int i = 0; i < 20; i++)
                {
                    DoWork();
                }
                logger.LogInformation($"Process task finished!");
                Monitor.Exit(syncRoot);
            }
            else
            {
                logger.LogInformation("Processing in progress. Skipped!");
            }
        }

        private void DoWork()
        {
            var number = random.Next(23);

            var processor = services.GetRequiredService<TaskHandler>();
            var queue = services.GetRequiredService<IBackgroundTaskQueue>();

            queue.QueueBackgroundItem(token =>
            {
                return processor.RunAsync(number, token);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}

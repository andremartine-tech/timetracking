using VialoginTestTimeTrackingAPI.Application.Interfaces;

namespace VialoginTestTimeTrackingAPI.Infrastructure.BackgroundJobs
{
    public class BackgroundTaskWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public BackgroundTaskWorker(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Erro ao processar tarefa: {ex.Message}");
                }
            }
        }
    }

}

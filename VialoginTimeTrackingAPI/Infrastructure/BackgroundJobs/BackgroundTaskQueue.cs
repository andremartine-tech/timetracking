using System.Threading.Channels;
using VialoginTestTimeTrackingAPI.Application.Interfaces;

namespace VialoginTestTimeTrackingAPI.Infrastructure.BackgroundJobs
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public BackgroundTaskQueue(int capacity = 100)
        {
            _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(capacity);
        }

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (!_queue.Writer.TryWrite(workItem))
            {
                throw new InvalidOperationException("A fila de tarefas está cheia.");
            }
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}

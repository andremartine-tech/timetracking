using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IProcessMessageUpdateTimeLogUseCase
    {
        Task ExecuteAsync(string message, CancellationToken cancellationToken);
    }
}

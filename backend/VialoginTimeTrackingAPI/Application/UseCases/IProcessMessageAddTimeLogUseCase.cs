using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IProcessMessageAddTimeLogUseCase
    {
        Task ExecuteAsync(string message, CancellationToken cancellationToken);
    }
}

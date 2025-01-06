namespace Application.UseCases
{
    public interface IProcessMessageDeleteTimeLogUseCase
    {
        Task ExecuteAsync(string message, CancellationToken cancellationToken);
    }
}

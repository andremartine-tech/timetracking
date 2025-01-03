namespace Application.UseCases
{
    public interface IProcessMessageAddUserUseCase
    {
        Task ExecuteAsync(string message, CancellationToken cancellationToken);
    }
}

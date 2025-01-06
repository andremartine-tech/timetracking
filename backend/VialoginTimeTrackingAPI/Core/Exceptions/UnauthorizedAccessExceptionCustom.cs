namespace VialoginTimeTrackingAPI.Core.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros de autorização de acesso.
    /// </summary>
    public class UnauthorizedAccessExceptionCustom : Exception
    {
        /// <summary>
        /// Construtor padrão com mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public UnauthorizedAccessExceptionCustom(string message) : base(message) { }

        /// <summary>
        /// Construtor com mensagem de erro e exceção interna.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="innerException">Exceção interna.</param>
        public UnauthorizedAccessExceptionCustom(string message, Exception innerException) : base(message, innerException) { }
    }
}

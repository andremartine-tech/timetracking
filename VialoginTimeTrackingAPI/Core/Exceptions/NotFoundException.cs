using System;

namespace Core.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros de validação de dados.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Construtor padrão com mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Construtor com mensagem de erro e exceção interna.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="innerException">Exceção interna.</param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

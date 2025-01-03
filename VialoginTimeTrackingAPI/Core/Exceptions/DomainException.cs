using System;

namespace Core.Exceptions
{
    /// <summary>
    /// Exceção personalizada para erros relacionados às regras de negócio do domínio.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Construtor padrão com mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Construtor com mensagem de erro e exceção interna.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="innerException">Exceção interna.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}

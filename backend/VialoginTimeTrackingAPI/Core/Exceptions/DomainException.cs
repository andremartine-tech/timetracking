using System;

namespace Core.Exceptions
{
    /// <summary>
    /// Exce��o personalizada para erros relacionados �s regras de neg�cio do dom�nio.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Construtor padr�o com mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public DomainException(string message) : base(message) { }

        /// <summary>
        /// Construtor com mensagem de erro e exce��o interna.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="innerException">Exce��o interna.</param>
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}

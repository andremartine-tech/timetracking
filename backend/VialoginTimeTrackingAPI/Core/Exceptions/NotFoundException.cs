using System;

namespace Core.Exceptions
{
    /// <summary>
    /// Exce��o personalizada para erros de valida��o de dados.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Construtor padr�o com mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        public NotFoundException(string message) : base(message) { }

        /// <summary>
        /// Construtor com mensagem de erro e exce��o interna.
        /// </summary>
        /// <param name="message">Mensagem descrevendo o erro.</param>
        /// <param name="innerException">Exce��o interna.</param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}

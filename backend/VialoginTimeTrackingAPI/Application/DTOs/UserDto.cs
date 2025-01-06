using System;

namespace Application.DTOs
{
    /// <summary>
    /// Objeto de transferência de dados para informações do usuário.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de usuário.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        public string Password { get; set; }
    }
}

using System;

namespace Application.DTOs
{
    /// <summary>
    /// Objeto de transfer�ncia de dados para informa��es do usu�rio.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador �nico do usu�rio.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de usu�rio.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usu�rio.
        /// </summary>
        public string Password { get; set; }
    }
}

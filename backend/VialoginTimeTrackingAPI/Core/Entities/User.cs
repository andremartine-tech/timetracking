using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class User
    {
        /// <summary>
        /// Identificador �nico do usu�rio.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de usu�rio (login).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usu�rio armazenada como hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Registros de ponto associados ao usu�rio.
        /// </summary>
        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}

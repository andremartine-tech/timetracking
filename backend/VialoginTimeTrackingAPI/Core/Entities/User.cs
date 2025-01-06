using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class User
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de usuário (login).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usuário armazenada como hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Registros de ponto associados ao usuário.
        /// </summary>
        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}

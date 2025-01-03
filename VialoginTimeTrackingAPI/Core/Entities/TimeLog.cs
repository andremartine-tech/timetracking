using System;

namespace Core.Entities
{
    public class TimeLog
    {
        /// <summary>
        /// Identificador �nico do registro de ponto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador do usu�rio associado ao registro de ponto.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de entrada.
        /// </summary>
        public DateTime TimestampIn { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de sa�da.
        /// </summary>
        public DateTime TimestampOut { get; set; }

        /// <summary>
        /// Refer�ncia para o usu�rio associado.
        /// </summary>
        public User? User { get; set; }
    }
}

using System;

namespace Core.Entities
{
    public class TimeLog
    {
        /// <summary>
        /// Identificador único do registro de ponto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador do usuário associado ao registro de ponto.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de entrada.
        /// </summary>
        public DateTime TimestampIn { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de saída.
        /// </summary>
        public DateTime TimestampOut { get; set; }

        /// <summary>
        /// Referência para o usuário associado.
        /// </summary>
        public User? User { get; set; }
    }
}

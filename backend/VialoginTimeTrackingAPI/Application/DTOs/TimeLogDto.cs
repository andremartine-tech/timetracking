namespace Application.DTOs
{
    /// <summary>
    /// Objeto de transferência de dados para registros de ponto.
    /// </summary>
    public class TimeLogDto
    {
        /// <summary>
        /// Identificador único do registro de ponto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador do usuário associado ao registro.
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
    }
}

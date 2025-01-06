namespace Application.DTOs
{
    /// <summary>
    /// Objeto de transfer�ncia de dados para registros de ponto.
    /// </summary>
    public class TimeLogDto
    {
        /// <summary>
        /// Identificador �nico do registro de ponto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador do usu�rio associado ao registro.
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
    }
}

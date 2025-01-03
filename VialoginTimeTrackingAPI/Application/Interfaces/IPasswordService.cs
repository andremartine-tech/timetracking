namespace Application.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a senhas.
    /// </summary>
    public interface IPasswordService
    {

        /// <summary>
        /// Gera um hash para a senha fornecida.
        /// </summary>
        /// <param name="password">Senha original.</param>
        /// <returns>Hash seguro da senha.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Compara uma senha em texto com o hash armazenado.
        /// </summary>
        /// <param name="plainPassword">Senha fornecida pelo usuário.</param>
        /// <param name="hashedPassword">Hash armazenado no banco de dados.</param>
        /// <returns>Verdadeiro se as senhas corresponderem; caso contrário, falso.</returns>
        bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}
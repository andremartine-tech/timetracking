using Application.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável por manipular senhas de forma segura.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Gera um hash para a senha fornecida.
        /// </summary>
        /// <param name="password">Senha original.</param>
        /// <returns>Hash seguro da senha.</returns>
        public string HashPassword(string password)
        {
            // Gera um hash seguro usando um fator de custo padrão (10)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Compara uma senha em texto com o hash armazenado.
        /// </summary>
        /// <param name="plainPassword">Senha fornecida pelo usuário.</param>
        /// <param name="hashedPassword">Hash armazenado no banco de dados.</param>
        /// <returns>Verdadeiro se as senhas corresponderem; caso contrário, falso.</returns>
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            // Verifica se a senha corresponde ao hash
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}


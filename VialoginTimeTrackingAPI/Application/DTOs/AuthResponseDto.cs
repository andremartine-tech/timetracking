using Application.DTOs;

namespace VialoginTimeTrackingAPI.Application.DTOs
{
    public class AuthResponseDto
    {
        /// <summary>
        /// UserDto do usuário logado.
        /// </summary>
        public UserDto User { get; set; }

        /// <summary>
        /// Token JWT.
        /// </summary>
        public string Token { get; set; }
    }
}

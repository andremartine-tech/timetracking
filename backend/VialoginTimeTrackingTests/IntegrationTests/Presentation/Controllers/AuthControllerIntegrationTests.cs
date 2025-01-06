using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using VialoginTimeTrackingAPI.Application.DTOs;
using Xunit;

namespace VialoginTimeTrackingTests.IntegrationTests.Presentation.Controllers
{
    

    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginCommand = new { Username = "test", Password = "test" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Auth/login", loginCommand);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(data.User);
            Assert.NotNull(data.Token);
            Assert.Equal("test", data.User.Username);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginCommand = new { Username = "invalidUser", Password = "wrongPassword" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_MissingFields_ReturnsBadRequest()
        {
            // Arrange
            var loginCommand = new { Username = "test" }; // Sem o campo Password

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalidToken");

            // Act
            var response = await _client.GetAsync("/api/Users");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_ValidToken_ReturnsOk()
        {
            // Arrange
            var loginCommand = new { Username = "test", Password = "test" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
            loginResponse.EnsureSuccessStatusCode();

            var loginData = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginData.Token);

            // Act
            var response = await _client.GetAsync("/api/Users");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_ExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            var expiredToken = "expired.jwt.token"; // Use um token expirado para o teste
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);

            // Act
            var response = await _client.GetAsync("/api/Users");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Logout_ValidToken_TokenBecomesInvalid()
        {
            // Arrange
            var loginCommand = new { Username = "test", Password = "test" };
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
            loginResponse.EnsureSuccessStatusCode();

            var loginData = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginData.Token);

            // Act - Logout
            var logoutResponse = await _client.PostAsync("/api/auth/logout", null);
            logoutResponse.EnsureSuccessStatusCode();

            // Act - Tentar acessar com o token antigo
            var protectedResponse = await _client.GetAsync("/api/Users");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
        }

        [Fact]
        public async Task RegisterAndLogin_NewUser_ReturnsToken()
        {
            // Arrange
            var loginCommand1 = new { Username = "test", Password = "test" };
            var loginResponse1 = await _client.PostAsJsonAsync("/api/auth/login", loginCommand1);
            //loginResponse1.EnsureSuccessStatusCode();

            var loginData1 = await loginResponse1.Content.ReadFromJsonAsync<AuthResponseDto>();

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginData1.Token);
            var registerCommand = new { Username = "newUser", Password = "password123" };
            var registerResponse = await _client.PostAsJsonAsync("/api/Users", registerCommand);
            registerResponse.EnsureSuccessStatusCode();

            var loginCommand2 = new { Username = "newUser", Password = "password123" };

            // Act
            var loginResponse2 = await _client.PostAsJsonAsync("/api/auth/login", loginCommand2);
            //loginResponse2.EnsureSuccessStatusCode();

            var loginData2 = await loginResponse2.Content.ReadFromJsonAsync<AuthResponseDto>();

            // Assert
            Assert.NotNull(loginData2);
            Assert.NotNull(loginData2.Token);
            Assert.Equal("newUser", loginData2.User.Username);
        }
    }

}

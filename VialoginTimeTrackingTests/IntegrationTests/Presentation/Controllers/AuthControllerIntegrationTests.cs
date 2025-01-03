namespace VialoginTimeTrackingTests.IntegrationTests.Presentation.Controllers
{
    using Xunit;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.VisualStudio.TestPlatform.TestHost;

    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder => { }).CreateClient();
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginCommand = new { Username = "test", Password = "password" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<dynamic>();

            // Assert
            Assert.NotNull(data);
            Assert.NotNull(data?.Token);
            Assert.Equal("test", (string)data?.User.Username);
        }
    }

}

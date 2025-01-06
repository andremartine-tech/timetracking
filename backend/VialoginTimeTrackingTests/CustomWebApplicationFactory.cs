using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VialoginTimeTrackingAPI.Core.Interfaces;

namespace VialoginTimeTrackingTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o serviço existente
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IRevokedTokensRepository));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona um mock para IRevokedTokensRepository
                var mockRevokedTokensRepository = new Mock<IRevokedTokensRepository>();
                mockRevokedTokensRepository
                    .Setup(repo => repo.IsTokenRevokedAsync(It.IsAny<string>()))
                    .ReturnsAsync(true); // Sempre retorna que o token não está revogado

                services.AddScoped(provider => mockRevokedTokensRepository.Object);
            });
        }
    }

}

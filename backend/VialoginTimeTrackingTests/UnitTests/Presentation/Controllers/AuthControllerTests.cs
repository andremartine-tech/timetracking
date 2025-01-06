using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Application.Features.Users.Commands;
using MediatR;
using Application.DTOs;
using System.Security.Claims;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Xunit;
using Application.Features.Users.Queries;
using VialoginTimeTrackingAPI.Application.DTOs;
using System;
using System.Threading;

namespace VialoginTimeTrackingTests.UnitTests.Presentation.Controllers
{
    public class AuthControllerUnitTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerUnitTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthController(_mediatorMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var username = "test";
            var password = "test";
            var command = new LoginUserCommand { Username = username, Password = password };
            var user = new UserDto { Id = Guid.NewGuid(), Username = username };
            var token = "sample.jwt.token";

            var authResponse = new AuthResponseDto
            {
                User = user,
                Token = token
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(command) as OkObjectResult;
            

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var responseData = result.Value as AuthResponseDto;

            Assert.Equal(username, responseData?.User.Username);
            Assert.Equal(token, responseData?.Token);
        }
    }
}

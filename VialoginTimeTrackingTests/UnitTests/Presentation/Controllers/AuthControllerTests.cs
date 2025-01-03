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
            var username = "unitTest";
            var password = "unitTest";
            var command = new LoginUserCommand { Username = username, Password = password };
            var user = new UserDto { Id = Guid.NewGuid(), Username = username };
            var token = "sample.jwt.token";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
                .ReturnsAsync((user, token));

            // Act
            var result = await _controller.Login(command) as OkObjectResult;
            

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var responseData = result.Value as AuthResponseDto;

            Assert.Equal(username, responseData?.User.Username);
            Assert.Equal(token, responseData?.Token);
        }

        [Fact]
        public async Task Verify_ValidUser_ReturnsOkWithUserAndToken()
        {
            // Arrange
            var username = "unitTest";
            var user = new UserDto { Id = Guid.NewGuid(), Username = username };
            var token = "sample.jwt.token";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetUserByUsernameQuery>(), default))
                .ReturnsAsync(user);

            _authServiceMock
                .Setup(a => a.GenerateJwtToken(It.IsAny<UserDto>()))
                .Returns(token);

            var httpContextMock = new DefaultHttpContext();
            httpContextMock.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock
            };

            // Act
            var result = await _controller.Verify() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            string json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            AuthResponseDto responseData = System.Text.Json.JsonSerializer.Deserialize<AuthResponseDto>(json);

            Assert.Equal(username, responseData?.User.Username);
            Assert.Equal(token, responseData?.Token);
        }
    }
}

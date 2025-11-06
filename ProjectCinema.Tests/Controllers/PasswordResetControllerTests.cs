using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Controllers;
using Xunit;

namespace ProjectCinema.Tests.Controllers
{
    public class PasswordResetControllerTests
    {
        private readonly Mock<IPasswordResetService> _serviceMock;
        private readonly Mock<IValidator<PasswordResetRequestDTO>> _requestValidatorMock;
        private readonly Mock<IValidator<PasswordResetConfirmDTO>> _confirmValidatorMock;
        private readonly PasswordResetController _controller;

        public PasswordResetControllerTests()
        {
            _serviceMock = new Mock<IPasswordResetService>(MockBehavior.Strict);
            _requestValidatorMock = new Mock<IValidator<PasswordResetRequestDTO>>(MockBehavior.Strict);
            _confirmValidatorMock = new Mock<IValidator<PasswordResetConfirmDTO>>(MockBehavior.Strict);

            _controller = new PasswordResetController(
                _serviceMock.Object,
                _requestValidatorMock.Object,
                _confirmValidatorMock.Object
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            httpContext.Request.Headers["User-Agent"] = "UnitTestAgent";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        private static ValidationResult Valid() => new ValidationResult();
        private static ValidationResult Invalid(string property, string message) =>
            new ValidationResult(new[] { new ValidationFailure(property, message) });

        [Fact]
        public async Task Request_ReturnsAccepted_OnValidRequest_CallsServiceWithIpAndUa()
        {
            var dto = new PasswordResetRequestDTO { Email = "user@example.com" };
            _requestValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Valid());
            _serviceMock.Setup(s => s.RequestPasswordResetAsync(dto, "127.0.0.1", "UnitTestAgent")).Returns(Task.CompletedTask);

            var result = await _controller.RequestPasswordResetAsync(dto);

            var accepted = Assert.IsType<AcceptedResult>(result);
            Assert.Equal(202, accepted.StatusCode);
            _serviceMock.VerifyAll();
        }

        [Fact]
        public async Task Request_ReturnsBadRequest_OnInvalidDto()
        {
            var dto = new PasswordResetRequestDTO { Email = "bad" };
            _requestValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Invalid("Email", "Invalid"));

            var result = await _controller.RequestPasswordResetAsync(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
            _serviceMock.Verify(s => s.RequestPasswordResetAsync(It.IsAny<PasswordResetRequestDTO>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Request_Returns500_OnServiceException()
        {
            var dto = new PasswordResetRequestDTO { Email = "user@example.com" };
            _requestValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Valid());
            _serviceMock.Setup(s => s.RequestPasswordResetAsync(dto, "127.0.0.1", "UnitTestAgent")).ThrowsAsync(new Exception("fail"));

            var result = await _controller.RequestPasswordResetAsync(dto);

            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task Validate_ReturnsOk_WhenTokenValid()
        {
            var token = "t";
            _serviceMock.Setup(s => s.ValidateTokenAsync(token)).ReturnsAsync(true);

            var result = await _controller.ValidateTokenAsync(token);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task Validate_Returns400_WhenTokenMissing()
        {
            var result = await _controller.ValidateTokenAsync("");
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
            _serviceMock.Verify(s => s.ValidateTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Validate_Returns410_WhenTokenInvalid()
        {
            var token = "t";
            _serviceMock.Setup(s => s.ValidateTokenAsync(token)).ReturnsAsync(false);

            var result = await _controller.ValidateTokenAsync(token);

            var gone = Assert.IsType<ObjectResult>(result);
            Assert.Equal(410, gone.StatusCode);
        }

        [Fact]
        public async Task Validate_Returns500_OnServiceException()
        {
            var token = "t";
            _serviceMock.Setup(s => s.ValidateTokenAsync(token)).ThrowsAsync(new Exception("x"));

            var result = await _controller.ValidateTokenAsync(token);
            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }

        [Fact]
        public async Task Confirm_ReturnsOk_OnSuccess()
        {
            var dto = new PasswordResetConfirmDTO { Token = "t", NewPassword = "Abcdef1!", ConfirmPassword = "Abcdef1!" };
            _confirmValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Valid());
            _serviceMock.Setup(s => s.ConfirmPasswordResetAsync(dto)).ReturnsAsync(true);

            var result = await _controller.ConfirmPasswordResetAsync(dto);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task Confirm_ReturnsBadRequest_OnInvalidDto()
        {
            var dto = new PasswordResetConfirmDTO { Token = "", NewPassword = "x", ConfirmPassword = "y" };
            _confirmValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Invalid("Token", "Invalid"));

            var result = await _controller.ConfirmPasswordResetAsync(dto);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
            _serviceMock.Verify(s => s.ConfirmPasswordResetAsync(It.IsAny<PasswordResetConfirmDTO>()), Times.Never);
        }

        [Fact]
        public async Task Confirm_Returns410_WhenServiceReturnsFalse()
        {
            var dto = new PasswordResetConfirmDTO { Token = "t", NewPassword = "Abcdef1!", ConfirmPassword = "Abcdef1!" };
            _confirmValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Valid());
            _serviceMock.Setup(s => s.ConfirmPasswordResetAsync(dto)).ReturnsAsync(false);

            var result = await _controller.ConfirmPasswordResetAsync(dto);
            var gone = Assert.IsType<ObjectResult>(result);
            Assert.Equal(410, gone.StatusCode);
        }

        [Fact]
        public async Task Confirm_Returns500_OnServiceException()
        {
            var dto = new PasswordResetConfirmDTO { Token = "t", NewPassword = "Abcdef1!", ConfirmPassword = "Abcdef1!" };
            _confirmValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(Valid());
            _serviceMock.Setup(s => s.ConfirmPasswordResetAsync(dto)).ThrowsAsync(new Exception("x"));

            var result = await _controller.ConfirmPasswordResetAsync(dto);
            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);
        }
    }
}



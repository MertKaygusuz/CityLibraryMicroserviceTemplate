using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserServiceApi.Controllers.User;
using UserServiceApi.Dtos.User;
using UserServiceApi.Services.User.Interfaces;

namespace UserServiceApi.UnitTests;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Fixture _fixture;


    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task Register_ValidDto_ReturnsUserId()
    {
        // Arrange
        var dto = _fixture.Create<RegistrationDto>();
        _userServiceMock.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(Guid.NewGuid().ToString());

        // Act
        var result = await _controller.Register(dto);

        // Assert
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task AdminUpdateUser_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<AdminUserUpdateDto>();

        // Act
        var result = await _controller.AdminUpdateUser(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UserSelfUpdate_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = new UserSelfUpdateDto();

        // Act
        var result = await _controller.UserSelfUpdate(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}
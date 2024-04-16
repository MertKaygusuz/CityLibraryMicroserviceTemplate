using AutoFixture;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.MapperConfigurations;
using CityLibrary.Shared.SharedModels.QueueModels;
using MassTransit;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Moq;
using UserServiceApi.AppSettings;
using UserServiceApi.Dtos.User;
using UserServiceApi.Entities;
using UserServiceApi.Repositories;
using UserServiceApi.Resources;
using System.Linq.Expressions;
using MockQueryable.Moq;
using UserServiceApi.Services.User.Classes;
using CityLibrary.Shared.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace UserServiceApi.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUsersRepo> _usersRepoMock;
    private readonly Mock<IRolesRepo> _rolesRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomMapper> _mapperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ISendEndpointProvider> _sendEndpointProviderMock;
    private readonly Mock<IOptions<AppSetting>> _optionsMock;
    private readonly Mock<IStringLocalizer<ExceptionsResource>> _localizerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UserService _userService;
    private readonly Fixture _fixture;

    public UserServiceTests()
    {
        _usersRepoMock = new Mock<IUsersRepo>();
        _rolesRepoMock = new Mock<IRolesRepo>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<ICustomMapper>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _sendEndpointProviderMock = new Mock<ISendEndpointProvider>();
        _optionsMock = new Mock<IOptions<AppSetting>>();
        _localizerMock = new Mock<IStringLocalizer<ExceptionsResource>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userService = new UserService(
            _usersRepoMock.Object,
            _rolesRepoMock.Object,
            _localizerMock.Object,
            _unitOfWorkMock.Object,
            _optionsMock.Object,
            _sendEndpointProviderMock.Object,
            _publishEndpointMock.Object,
            _mapperMock.Object,
            _httpContextAccessorMock.Object
        );
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }


    [Fact]
    public async Task GetUserByUserNameAsync_WhenUserNameExists_ShouldReturnUser()
    {
        // Arrange
        string userName = "testuser";
        var expectedUser = _fixture.Create<User>();
        expectedUser.UserName = userName;
        var queryableUser = new List<User> { expectedUser }.AsQueryable().BuildMock();
        _usersRepoMock.Setup(repo => repo.GetDataWithLinqExp(x => x.UserName == userName, "Roles"))
                        .Returns(queryableUser);

        // Act
        var result = await _userService.GetUserByUserNameAsync(userName);

        // Assert
        Assert.Equal(expectedUser, result);
        Assert.Equal(expectedUser.UserName, userName);
    }

    [Fact]
    public async Task GetUserByUserNameAsync_WhenUserNameDoesExists_ShouldReturnNull()
    {
        // Arrange
        string userName = "testuser";
        User expectedUser = null;
        var queryableUser = new List<User> { }.AsQueryable().BuildMock();
        _usersRepoMock.Setup(repo => repo.GetDataWithLinqExp(x => x.UserName == userName, "Roles"))
                        .Returns(queryableUser);

        // Act
        var result = await _userService.GetUserByUserNameAsync(userName);

        // Assert
        Assert.Null(expectedUser);
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationDtoIsValid_ShouldReturnUserId()
    {
        // Arrange
        var registrationDto = _fixture.Create<RegistrationDto>();
        var newUser = _fixture.Create<User>();
        var options = _fixture.Create<AppSetting>();
        options.RabbitMqOptions.UserCreatedSenderUri = "queue:fake-it-up-shekerim";

        _mapperMock.Setup(mapper => mapper.Map<RegistrationDto, User>(registrationDto))
            .Returns(newUser);
        _usersRepoMock.Setup(repo => repo.InsertAsync(newUser))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(mapper => mapper.Map<User, UserCreated>(newUser))
            .Returns(_fixture.Create<UserCreated>());
        _optionsMock.Setup(op => op.Value).Returns(options);
        _sendEndpointProviderMock.Setup(provider => provider.GetSendEndpoint(It.IsAny<Uri>()))
            .ReturnsAsync(Mock.Of<ISendEndpoint>());

        // Act
        var result = await _userService.RegisterAsync(registrationDto);

        // Assert
        Assert.Equal(newUser.UserId, result);
        _usersRepoMock.Verify(repo => repo.InsertAsync(newUser), Times.Once);
        _rolesRepoMock.Verify(repo => repo.SetUserRolesWithLinqExp(It.IsAny<User>(), It.IsAny<Expression<Func<Role, bool>>>()), Times.Once);
        _sendEndpointProviderMock.Verify(sep => sep.GetSendEndpoint(It.IsAny<Uri>()), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UserSelfUpdateAsync_ValidUserId_CompletedTask()
    {
        // Arrange
        var fixture = new Fixture();
        var selfUpdateDto = fixture.Create<UserSelfUpdateDto>();
        var myUserId = "myUserId";
        var adminUserUpdateDto = fixture.Create<AdminUserUpdateDto>();
        _userService.AdminUpdateUserFunc = (AdminUserUpdateDto) => Task.CompletedTask;

        _httpContextAccessorMock.Setup(x => x.HttpContext.User.Claims)
            .Returns([new (ClaimTypes.Sid, myUserId)]);

        _usersRepoMock.Setup(x => x.DoesEntityExistAsync(myUserId))
            .ReturnsAsync(true);

        _mapperMock.Setup(x => x.Map<UserSelfUpdateDto, AdminUserUpdateDto>(selfUpdateDto))
            .Returns(adminUserUpdateDto);

        // Act
        await _userService.UserSelfUpdateAsync(selfUpdateDto);
        
        // Assert
        _mapperMock.Verify(mapper => mapper.Map<UserSelfUpdateDto, AdminUserUpdateDto>(selfUpdateDto), Times.Once);
        _usersRepoMock.Verify(repo => repo.DoesEntityExistAsync(myUserId), Times.Once);
    }

    [Fact]
    public async Task UserSelfUpdateAsync_InvalidUserId_ThrowsCustomBusinessException()
    {
        // Arrange
        var fixture = new Fixture();
        var selfUpdateDto = fixture.Create<UserSelfUpdateDto>();
        var myUserId = "myUserId";

        _httpContextAccessorMock.Setup(x => x.HttpContext.User.Claims)
            .Returns([new Claim(ClaimTypes.Sid, myUserId)]);

        _usersRepoMock.Setup(x => x.DoesEntityExistAsync(myUserId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<CustomBusinessException>(() => _userService.UserSelfUpdateAsync(selfUpdateDto));
    }

    [Fact]
    public async Task AdminUpdateUserAsync_WithValidDto_UpdatesUserAndPublishesEvent()
    {
        // Arrange
        var updateDto = _fixture.Create<AdminUserUpdateDto>();
        var user = _fixture.Create<User>();
        _usersRepoMock.Setup(repo => repo.GetByIdAsync(updateDto.UserId)).ReturnsAsync(user);

        // Act
        await _userService.AdminUpdateUserAsync(updateDto);

        // Assert
        _usersRepoMock.Verify(repo => repo.GetByIdAsync(updateDto.UserId), Times.Once);
        _publishEndpointMock.Verify(publishEndpoint => publishEndpoint.Publish(It.IsAny<UserUpdated>(), default), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
    }

}
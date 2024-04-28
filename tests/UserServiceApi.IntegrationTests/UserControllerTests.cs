using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserServiceApi.AppSettings;
using UserServiceApi.ContextRelated;
using UserServiceApi.Dtos.User;
using UserServiceApi.IntegrationTests.Fixtures;
using UserServiceApi.IntegrationTests.Helpers;

namespace UserServiceApi.IntegrationTests;

[Collection("Shared Collection")]
public class UserControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string baseUrl = "api/User/";
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbHelpers.ReinitDbForTests(db);
    }

    public UserControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidDto_ShouldReturnAValidGuid()
    {
        // arrange
        var dto = new RegistrationDto() 
        {
            UserName = "NewUser",
            FullName = "NewUserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Address = "Address",
            Password = "1234567890"
        };
        // _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser());

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "Register", dto);

        // assert
        response.EnsureSuccessStatusCode();
        string cretaedUserId = await response.Content.ReadAsStringAsync();
        var isValidGuid = Guid.TryParse(cretaedUserId, out _);
        Assert.True(isValidGuid);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(5, db.Users.Count()); 
    }

    [Fact]
    public async Task Register_WithExistingUserName_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new RegistrationDto() 
        {
            UserName = "Admin",
            FullName = "NewUserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Address = "Address",
            Password = "1234567890"
        };
        // _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser());

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "Register", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AdminUpdateUser_WithValidDto_ShouldUpdateTheUser()
    {
        // arrange
        var dto = new AdminUserUpdateDto() 
        {
            UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "AdminUpdateUser", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var upatedUser = await db.Users.FindAsync(dto.UserId);
        Assert.Equal(upatedUser.FullName, dto.FullName);
        Assert.Equal(upatedUser.Address, dto.Address);
        Assert.Equal(upatedUser.BirthDate, dto.BirthDate);
    }

    [Fact]
    public async Task AdminUpdateUser_WithUserIdWhichDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var dto = new AdminUserUpdateDto() 
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "AdminUpdateUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminUpdateUser_WithInValidToken_ShouldReturnForbidden()
    {
        // arrange
        var dto = new AdminUserUpdateDto() 
        {
            UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetUserToken(options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "AdminUpdateUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminUpdateUser_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var dto = new AdminUserUpdateDto() 
        {
            UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "AdminUpdateUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserSelfUpdate_WithValidDto_ShouldUpdateTheUser()
    {
        // arrange
        var defaultUserId = "75a4749d-1090-4ade-894e-2612adcd0c1c";
        var dto = new UserSelfUpdateDto() 
        {
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetUserToken(options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "UserSelfUpdate", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var upatedUser = await db.Users.FindAsync(defaultUserId);
        Assert.Equal(upatedUser.FullName, dto.FullName);
        Assert.Equal(upatedUser.Address, dto.Address);
        Assert.Equal(upatedUser.BirthDate, dto.BirthDate);
    }

    [Fact]
    public async Task UserSelfUpdate_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var dto = new UserSelfUpdateDto() 
        {
            FullName = "New UserFullName",
            BirthDate = DateTime.UtcNow.AddYears(-32),
            Address = "New Address",
            Password = "1234567890"
        };

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "UserSelfUpdate", dto);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
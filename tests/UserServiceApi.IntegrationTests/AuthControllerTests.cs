using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UserServiceApi.ContextRelated;
using UserServiceApi.Dtos.Authentication;
using UserServiceApi.Dtos.Token.Records;
using UserServiceApi.IntegrationTests.Fixtures;
using UserServiceApi.IntegrationTests.Helpers;

namespace UserServiceApi.IntegrationTests;

[Collection("Shared Collection")]
public class AuthControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string baseUrl = "api/Auth/";
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbHelpers.ReinitDbForTests(db);
    }

    public AuthControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokenRecord()
    {
        // arrange
        var dto = new LoginDto() 
        {
            UserName = "Admin",
            Password = "1234567890"
        };

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "Login", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseVal = await response.Content.ReadFromJsonAsync<ReturnTokenRecord>();
        Assert.IsType<ReturnTokenRecord>(responseVal);
    }

    [Fact]
    public async Task Login_WithInValidCredentials_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new LoginDto() 
        {
            UserName = "Admin",
            Password = "fakepassword"
        };

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "Login", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ReLogin_WithValidRefreshToken_ShouldReturnTokenRecord()
    {
        // arrange
        var dto = new LoginDto() 
        {
            UserName = "Admin",
            Password = "1234567890"
        };
        var responseInLogin = await _httpClient.PostAsJsonAsync(baseUrl + "Login", dto);
        var responseInLoginVal = await responseInLogin.Content.ReadFromJsonAsync<ReturnTokenRecord>();
        var formVariables = new List<KeyValuePair<string, string>>
        {
            new("refreshToken", responseInLoginVal.RefreshToken)
        };
        var formContent = new FormUrlEncodedContent(formVariables);

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "ReLoginWithRefreshToken", formContent);
        

        // assert
        var responseVal = await response.Content.ReadFromJsonAsync<ReturnTokenRecord>();
        Assert.IsType<ReturnTokenRecord>(responseVal);
    }
}


using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using UserServiceApi.ContextRelated;
using UserServiceApi.IntegrationTests.Fixtures;
using UserServiceApi.IntegrationTests.Helpers;

namespace UserServiceApi.IntegrationTests;

[Collection("Shared Collection")]
public class TestControllerTest : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbHelpers.ReinitDbForTests(db);
    }

    public TestControllerTest(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;

    [Fact]
    public async Task GetAllUsers_ShouldReturn4Users()
    {
        // act
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<Entities.User>>("api/Test/GetAllUsers");

        // assert
        Assert.Equal(4, response.Count());
    }
}
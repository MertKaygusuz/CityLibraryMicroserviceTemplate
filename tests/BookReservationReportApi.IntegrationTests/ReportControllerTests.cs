
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookReservationReportApi.AppSettings;
using BookReservationReportApi.Dtos.ReservationReport;
using BookReservationReportApi.IntegrationTests.Fixtures;
using BookReservationReportApi.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookReservationReportApi.IntegrationTests;

[Collection("Shared Collection")]
public class ReportControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string baseUrl = "api/Report/";
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    public ReportControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveBookReservations_WithNoFilter_ShouldReturn3Count()
    {
        // arrange
        var dto = new ActiveBookReservationsFilterDto();
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetActiveBookReservations", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<ActiveBookReservationsResponseDto>>();
        Assert.Equal(3, responseData.Count());
    }

    [Fact]
    public async Task GetActiveBookReservations_WithUserFilter_ShouldReturn1Count()
    {
        // arrange
        var dto = new ActiveBookReservationsFilterDto()
        {
            UserName = "User1"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetActiveBookReservations", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<ActiveBookReservationsResponseDto>>();
        Assert.Single(responseData);
    }

    [Fact]
    public async Task GetActiveBookReservations_WithBookFilter_ShouldReturn1Count()
    {
        // arrange
        var dto = new ActiveBookReservationsFilterDto()
        {
            BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetActiveBookReservations", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<ActiveBookReservationsResponseDto>>();
        Assert.Single(responseData);
    }

    [Fact]
    public async Task GetActiveBookReservations_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var dto = new ActiveBookReservationsFilterDto();

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetActiveBookReservations", dto);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveBookReservations_WithoutAdminToken_ShouldReturnForbidden()
    {
        // arrange
        var dto = new ActiveBookReservationsFilterDto();
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetUserToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetActiveBookReservations", dto);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetNumberOfBooksReservedPerUsers_ShouldReturn2Count()
    {
        // arrange
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<NumberOfBooksReservedByUsersResponseDto>>(baseUrl + "GetNumberOfBooksReservedPerUsers");

        // assert
        Assert.Equal(2, response.Count());
    }

    [Fact]
    public async Task GetReservationHistoryPerBook_ShouldReturn4Count()
    {
        // arrange
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ReservationHistoryBookResponseDto>>(baseUrl + "GetReservationHistoryPerBook");

        // assert
        Assert.Equal(4, response.Count());
    }

    [Fact]
    public async Task GetReservationHistoryPerUser_ShouldReturn4Count()
    {
        // arrange
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<ReservationHistoryBookResponseDto>>(baseUrl + "GetReservationHistoryPerUser");

        // assert
        Assert.Equal(4, response.Count());
    }

    [Fact]
    public async Task GetReservationHistoryByBook_WithBookId1_ShouldReturn2Count()
    {
        // arrange
        var dto = new ReservationHistoryBookDto()
        {
            BookId = 1
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetReservationHistoryByBook", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<ReservationHistoryBookResponseDto>>();
        Assert.Single(responseData);
    }

    [Fact]
    public async Task GetReservationHistoryByUser_WithValidUserId_ShouldReturn2Count()
    {
        // arrange
        var dto = new ReservationHistoryUserDto()
        {
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "GetReservationHistoryByUser", dto);

        // assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<IEnumerable<ReservationHistoryUserResponseDto>>();
        Assert.Equal(2, responseData.Count());
    }
}
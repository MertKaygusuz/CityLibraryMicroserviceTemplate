using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookReservationReportApi.AppSettings;
using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Entities;
using BookReservationReportApi.IntegrationTests.Fixtures;
using BookReservationReportApi.IntegrationTests.Helpers;
using CityLibrary.Shared.Extensions;
using CityLibrary.Shared.SharedEnums;
using CityLibrary.Shared.SharedModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace BookReservationReportApi.IntegrationTests;

[Collection("Shared Collection")]
public class RecordControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string baseUrl = "api/Record/";
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() 
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbHelpers.ReinitDbForTests(db);
    }

    public RecordControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CreateReservation_WithValidDto_ShouldInsertData()
    {
        // arrange
        var dto = new ActiveBookReservationModel() 
        {
            UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
            User = new() 
            {
                UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                UserName = "User2",
                FullName = "Kaya",
                BirthDate = DateTime.UtcNow.AddYears(-40),
                Address = "Kaya's Address"
            },
            BookId = 1,
            Book = new()
            {
                BookId = 1,
                BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni",
                Author = "Friedrich Engels",
                FirstPublishDate = DateTime.UtcNow.AddYears(-138),
                EditionNumber = 4,
                EditionDate = DateTime.UtcNow.AddYears(-120),
                TitleType = BookTitleTypes.Science,
                CoverType = BookCoverTypes.HardCover
            },
            DeliveryDateToUser = DateTime.UtcNow
        };
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "CreateReservation", dto);

        // assert
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var activeBookReservationCollection = context.Database.Collection<ActiveBookReservation>();
        var totalActiveReservationCount = await activeBookReservationCollection.CountDocumentsAsync(new BsonDocument());
        response.EnsureSuccessStatusCode();
        Assert.Equal(4, totalActiveReservationCount);
    }

    [Fact]
    public async Task CreateReservation_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var dto = new ActiveBookReservationModel() 
        {
            UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
            User = new() 
            {
                UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                UserName = "User2",
                FullName = "Kaya",
                BirthDate = DateTime.UtcNow.AddYears(-40),
                Address = "Kaya's Address"
            },
            BookId = 1,
            Book = new()
            {
                BookId = 1,
                BookTitle = "Ailenin, Devletin ve Özel Mülkiyetin Kökeni",
                Author = "Friedrich Engels",
                FirstPublishDate = DateTime.UtcNow.AddYears(-138),
                EditionNumber = 4,
                EditionDate = DateTime.UtcNow.AddYears(-120),
                TitleType = BookTitleTypes.Science,
                CoverType = BookCoverTypes.HardCover
            },
            DeliveryDateToUser = DateTime.UtcNow
        };

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "CreateReservation", dto);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

}

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookServiceApi.AppSettings;
using BookServiceApi.ContextRelated;
using BookServiceApi.Dtos.Book;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Entities;
using BookServiceApi.IntegrationTests.Fixtures;
using BookServiceApi.IntegrationTests.Helpers;
using BookServiceApi.IntegrationTests.Mocks;
using CityLibrary.Shared.SharedEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WireMock.Server;

namespace BookServiceApi.IntegrationTests;

[Collection("Shared Collection")]
public class BookControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private WireMockServer _bookReservationReportServer;
    private IOptions<AppSetting> _options;
    private const string baseUrl = "api/Book/";
    public Task InitializeAsync() 
    {
        using var scope = _factory.Services.CreateScope();
        _options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();

        _bookReservationReportServer = WireMockServer.Start(_options.Value.ReservationServiceBaseUrl);
        BookReservationApiEndPointMocks.CreateReservationStub(_bookReservationReportServer);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() 
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbHelpers.ReinitDbForTests(db);
        _bookReservationReportServer.Stop();
    }

    public BookControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task AssignBookToUser_WithNoAuth_ShouldReturn401()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AssignBookToUser_WithoutAdminToken_ShouldReturnForbidden()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetUserToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AssignBookToUser_WithValidDto_ShouldMakeAssignment()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var theBookToBeAssigned = await context.Books.FindAsync(dto.BookId);
        response.EnsureSuccessStatusCode();
        Assert.Equal(2, theBookToBeAssigned.AvailableCount);
        Assert.Equal(1, theBookToBeAssigned.ReservedCount);
    }

    [Fact]
    public async Task AssignBookToUser_WithInValidUser_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = Guid.NewGuid().ToString()
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AssignBookToUser_WithInValidBook_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 99999999,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AssignBookToUser_WithNotAvailableBook_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var theBook = await db.Books.FindAsync(dto.BookId);
        theBook.AvailableCount = 0;
        db.SaveChanges();

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "AssignBookToUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnAssignBookFromUser_WithValidDto_ShouldMakeUnAssignment()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));
        // set reserved count
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var theBook = await db.Books.FindAsync(dto.BookId);
        theBook.ReservedCount = 50;
        db.SaveChanges();

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "UnAssignBookFromUser", dto);

        // assert
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var theBookToBeUnAssigned = await context
                                            .Books
                                            .AsNoTracking()
                                            .Where(x => x.BookId == theBook.BookId)
                                            .Select(x => new 
                                            {
                                                x.AvailableCount,
                                                x.ReservedCount
                                            }).SingleOrDefaultAsync();
        response.EnsureSuccessStatusCode();
        Assert.Equal(4, theBookToBeUnAssigned.AvailableCount);
        Assert.Equal(49, theBookToBeUnAssigned.ReservedCount);
    }

    [Fact]
    public async Task UnAssignBookFromUser_WithInValidUser_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 1,
            UserId = Guid.NewGuid().ToString()
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "UnAssignBookFromUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnAssignBookFromUser_WithInValidBook_ShouldReturnBadRequest()
    {
        // arrange
        var dto = new AssignBookToUserDto() 
        {
            BookId = 99999999,
            UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3"
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "UnAssignBookFromUser", dto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllBooks_WithAdminToken_ShouldReturn5Books()
    {
        // arrange
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<Book>>(baseUrl + "GetAllBooks");

        // assert
        Assert.Equal(5, response.Count());
    }

    [Fact]
    public async Task BookRegister_WithValidDto_ShouldReturnNewBookId()
    {
        // arrange
        var dto = new RegisterBookDto() 
        {
            Author = "fake",
            BookTitle = "fake title",
            FirstPublishDate = DateTime.UtcNow.AddYears(-2),
            EditionNumber = 2,
            EditionDate = DateTime.UtcNow.AddYears(-1),
            TitleType = BookTitleTypes.Business,
            CoverType = BookCoverTypes.HardCover,
            AvailableCount = 2
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PostAsJsonAsync(baseUrl + "BookRegister", dto);

        // assert
        response.EnsureSuccessStatusCode();
        string cretaedBookId = await response.Content.ReadAsStringAsync();
        var isValidInt = int.TryParse(cretaedBookId, out _);
        Assert.True(isValidInt);
    }

    [Fact]
    public async Task BookInfoUpdate_WithValidDto_ShouldReturnUpdateBook()
    {
        // arrange
        var dto = new UpdateBookDto() 
        {
            BookId = 1,
            Author = "fake",
            BookTitle = "fake title",
            FirstPublishDate = DateTime.UtcNow.AddYears(-2),
            EditionNumber = 2,
            EditionDate = DateTime.UtcNow.AddYears(-1),
            TitleType = BookTitleTypes.Business,
            CoverType = BookCoverTypes.HardCover,
            AvailableCount = 2
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "BookInfoUpdate", dto);

        // assert
        response.EnsureSuccessStatusCode();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var upatedBook = await db.Books.FindAsync(dto.BookId);
        Assert.Equal(upatedBook.Author, dto.Author);
        Assert.Equal(upatedBook.BookTitle, dto.BookTitle);
        Assert.Equal(upatedBook.FirstPublishDate, dto.FirstPublishDate);
        Assert.Equal(upatedBook.EditionNumber, dto.EditionNumber);
        Assert.Equal(upatedBook.EditionDate, dto.EditionDate);
        Assert.Equal(upatedBook.TitleType, dto.TitleType);
        Assert.Equal(upatedBook.CoverType, dto.CoverType);
        Assert.Equal(upatedBook.AvailableCount, dto.AvailableCount);
    }

    [Fact]
    public async Task BookInfoUpdate_WithInValidBookId_ShouldReturnNotFound()
    {
        // arrange
        var dto = new UpdateBookDto() 
        {
            BookId = 99999999,
            Author = "fake",
            BookTitle = "fake title",
            FirstPublishDate = DateTime.UtcNow.AddYears(-2),
            EditionNumber = 2,
            EditionDate = DateTime.UtcNow.AddYears(-1),
            TitleType = BookTitleTypes.Business,
            CoverType = BookCoverTypes.HardCover,
            AvailableCount = 2
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));

        // act
        var response = await _httpClient.PutAsJsonAsync(baseUrl + "BookInfoUpdate", dto);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
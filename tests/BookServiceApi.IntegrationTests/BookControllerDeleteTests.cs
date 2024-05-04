using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookServiceApi.AppSettings;
using BookServiceApi.ContextRelated;
using BookServiceApi.Dtos.Book;
using BookServiceApi.IntegrationTests.Fixtures;
using BookServiceApi.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookServiceApi.IntegrationTests;

[Collection("Shared Delete Collection")]
public class BookControllerDeleteTests
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly IOptions<AppSetting> _options;

    public BookControllerDeleteTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        _options = scope.ServiceProvider.GetRequiredService<IOptions<AppSetting>>();
    }
    
    [Fact]
    public async Task DeleteBook_WithInValidBookId_ShouldReturnNotFound()
    {
        // arrange
        var dto = new DeleteBookDto() 
        {
            BookId = 99999999
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));
        HttpRequestMessage request = new()
        {
            Content = JsonContent.Create(dto),
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_options.Value.CurrentApplicationUrl  + "/DeleteBook")
        };

        // act
        var response = await _httpClient.SendAsync(request);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithValidBookId_ShouldDeleteTheBook()
    {
        // arrange
        var dto = new DeleteBookDto() 
        {
            BookId = 1
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthHelpers.GetAdminToken(_options));
        HttpRequestMessage request = new()
        {
            Content = JsonContent.Create(dto),
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_options.Value.CurrentApplicationUrl  + "/api/Book/DeleteBook")
        };

        // act
        var response = await _httpClient.SendAsync(request);

        // assert
        response.EnsureSuccessStatusCode();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var deletedBookDate = await db.Books
                                      .AsNoTracking()
                                      .IgnoreQueryFilters()
                                      .Where(x => x.BookId == dto.BookId)
                                      .Select(x => x.DeletedAt)
                                      .SingleOrDefaultAsync();
        Assert.NotNull(deletedBookDate);
    }
}
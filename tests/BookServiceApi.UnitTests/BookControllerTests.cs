using AutoFixture;
using BookServiceApi.Controllers.Book;
using BookServiceApi.Dtos.Book;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Services.Book.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookServiceApi.UnitTests;

public class BookControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly BookController _bookController;
    private readonly Fixture _fixture;

    public BookControllerTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _bookController = new BookController(_bookServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetAllBooks_ReturnsOkResult()
    {
        // Arrange
        var fixture = new Fixture();
        var expectedBooks = fixture.CreateMany<Entities.Book>();

        _bookServiceMock.Setup(x => x.GetAllBooks()).ReturnsAsync(expectedBooks);

        // Act
        var result = await _bookController.GetAllBooks();

        // Assert
        Assert.IsType<ActionResult<IEnumerable<Entities.Book>>>(result);
    }

    [Fact]
    public async Task BookRegister_WithValidRegisterBookDto_ReturnsRegisteredBookId()
    {
        // Arrange
        var fixture = new Fixture();
        var expectedBookId = fixture.Create<int>();
        var registerBookDto = fixture.Create<RegisterBookDto>();

        _bookServiceMock.Setup(x => x.BookRegisterAsync(registerBookDto)).ReturnsAsync(expectedBookId);

        // Act
        var result = await _bookController.BookRegister(registerBookDto);

        // Assert
        Assert.Equal(expectedBookId, result);
    }

    [Fact]
    public async Task BookInfoUpdate_WithValidUpdateBookDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<UpdateBookDto>();

        // Act
        var result = await _bookController.BookInfoUpdate(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithValidDeleteBookDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<DeleteBookDto>();

        // Act
        var result = await _bookController.DeleteBook(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task AssignBookToUser_WithValidAssignBookToUserDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<AssignBookToUserDto>();

        // Act
        var result = await _bookController.AssignBookToUser(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task UnAssignBookFromUser_WithValidAssignBookToUserDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<AssignBookToUserDto>();

        // Act
        var result = await _bookController.UnAssignBookFromUser(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}
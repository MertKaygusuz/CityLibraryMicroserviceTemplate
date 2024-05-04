using AutoFixture;
using BookServiceApi.Dtos.Book;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Repositories.Book;
using BookServiceApi.Repositories.User;
using BookServiceApi.Services.Book.Classes;
using BookServiceApi.Services.BookReservationApiService;
using BookServiceApi.Services.BookReservationApiService.Grpc;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.MapperConfigurations;
using CityLibrary.Shared.SharedModels;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using MockQueryable.Moq;
using BookServiceApi.AppSettings;
using System.Linq.Expressions;

namespace BookServiceApi.UnitTests;

public class BookServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IBooksRepo> _booksRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICustomMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IBookReservationRecordApi> _bookReservationRecordApiMock;
    private readonly Mock<IOptions<AppSetting>> _optionsMock;
    private readonly Mock<ISendEndpointProvider> _sendEndpointProviderMock;
    private readonly Mock<IBookReservationRecordApiGrpc> _bookReservationRecordApiGrpcMock;

    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _booksRepoMock = new Mock<IBooksRepo>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<ICustomMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _bookReservationRecordApiMock = new Mock<IBookReservationRecordApi>();
        _optionsMock = new Mock<IOptions<AppSetting>>();
        _sendEndpointProviderMock = new Mock<ISendEndpointProvider>();
        _bookReservationRecordApiGrpcMock = new Mock<IBookReservationRecordApiGrpc>();

        #region fixture configuration
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        #endregion

        #region lazy user repo mock configurations
        var usersRepoMock = new Mock<IUsersRepo>();
        var userRecord = _fixture.Create<Entities.User>();
        userRecord.BirthDate = userRecord.BirthDate.ToUniversalTime();
        usersRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(userRecord);
        var lazyUserRepoMock = new Lazy<IUsersRepo>(() => usersRepoMock.Object);
        #endregion

        _bookService = new BookService(
            _booksRepoMock.Object,
            _unitOfWorkMock.Object,
            lazyUserRepoMock,
            _mapperMock.Object,
            _optionsMock.Object,
            _sendEndpointProviderMock.Object,
            _bookReservationRecordApiMock.Object,
            _httpContextAccessorMock.Object,
            _bookReservationRecordApiGrpcMock.Object
        );
    }

    [Fact]
    public async Task BookRegisterAsync_WhenRegisterDtoValid_ShouldCallBooksRepoDeleteById()
    {
        // Arrange
        var dto = _fixture.Create<DeleteBookDto>();

        // Act
        await _bookService.DeleteBookAsync(dto);

        // Assert
        _booksRepoMock.Verify(r => r.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_DeleteBookDtoValid_ShouldInsertBookAndCommitUnitOfWork()
    {
        // Arrange
        var dto = _fixture.Create<RegisterBookDto>();
        var bookToAdd = _fixture.Create<Entities.Book>();
        _mapperMock.Setup(m => m.Map<RegisterBookDto, Entities.Book>(dto)).Returns(bookToAdd);

        // Act
        var result = await _bookService.BookRegisterAsync(dto);

        // Assert
        _booksRepoMock.Verify(r => r.InsertAsync(bookToAdd), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        Assert.Equal(bookToAdd.BookId, result);
    }

    [Fact]
    public async Task UpdateBookInfoAsync_UpdateBookDtoValid_ShouldUpdateBook()
    {
        // Arrange
        var dto = _fixture.Create<UpdateBookDto>();
        var options = _fixture.Create<AppSetting>();
        options.RabbitMqOptions.BookUpdatedSenderUri = "queue:fake-it-up-shekerim";
        _optionsMock.Setup(op => op.Value).Returns(options);
        _sendEndpointProviderMock.Setup(provider => provider.GetSendEndpoint(It.IsAny<Uri>()))
                                 .ReturnsAsync(Mock.Of<ISendEndpoint>());
        
        // Act
        await _bookService.UpdateBookInfoAsync(dto);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        _sendEndpointProviderMock.Verify(sep => sep.GetSendEndpoint(It.IsAny<Uri>()), Times.Once);
    }

    [Fact]
    public async Task AssignBookToUserAsync_AssignBookToUserDtoValid_ShouldUpdateBookRecordAndCallCreateReservation()
    {
        // Arrange
        var dto = _fixture.Create<AssignBookToUserDto>();
        var bookRecord = _fixture.Create<Entities.Book>();
        int initialBookReservedCount = bookRecord.ReservedCount;
        int initialBookAvailableCount = bookRecord.AvailableCount;
        _mapperMock.Setup(mapper => mapper.Map<Entities.User, UserModel>(It.IsAny<Entities.User>()))
                   .Returns(Mock.Of<UserModel>);
        _mapperMock.Setup(mapper => mapper.Map<Entities.Book, BookModel>(It.IsAny<Entities.Book>()))
                   .Returns(Mock.Of<BookModel>);
        _booksRepoMock.Setup(x => x.GetByIdAsync(dto.BookId)).ReturnsAsync(bookRecord);

        // Act
        await _bookService.AssignBookToUserAsync(dto);

        // Assert
        Assert.Equal(bookRecord.ReservedCount, initialBookReservedCount + 1);
        Assert.Equal(bookRecord.AvailableCount, initialBookAvailableCount - 1);
        _booksRepoMock.Verify(x => x.GetByIdAsync(dto.BookId), Times.Once);
        _bookReservationRecordApiMock.Verify(x => x.CreateReservation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ActiveBookReservationModel>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task UnAssignBookFromUserAsync_AssignBookToUserDtoValid_ShouldUpdateBookRecordAndCallGrpcClient()
    {
        // Arrange
        var dto = _fixture.Create<AssignBookToUserDto>();
        var bookRecord = _fixture.Create<Entities.Book>();
        bookRecord.FirstPublishDate = bookRecord.FirstPublishDate.ToUniversalTime();
        bookRecord.EditionDate = bookRecord.EditionDate.ToUniversalTime();

        int initialBookReservedCount = bookRecord.ReservedCount;
        int initialBookAvailableCount = bookRecord.AvailableCount;
        _booksRepoMock.Setup(x => x.GetByIdAsync(dto.BookId)).ReturnsAsync(bookRecord);
        _bookReservationRecordApiGrpcMock.Setup(x => x.CallReturnBookAsync(It.IsAny<Entities.User>(), It.IsAny<Entities.Book>())).Returns(Task.CompletedTask);

        // Act
        await _bookService.UnAssignBookFromUserAsync(dto);

        // Assert
        Assert.Equal(bookRecord.ReservedCount, initialBookReservedCount - 1);
        Assert.Equal(bookRecord.AvailableCount, initialBookAvailableCount + 1);
        _booksRepoMock.Verify(x => x.GetByIdAsync(dto.BookId), Times.Once);
        _bookReservationRecordApiGrpcMock.Verify(x => x.CallReturnBookAsync(It.IsAny<Entities.User>(), It.IsAny<Entities.Book>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CheckIfAnyAvailableBooksAsync_WhenAvailableBookCountGreaterThanZero_ShouldReturnTrue()
    {
        // Arrange
        var bookRecord = _fixture.Create<Entities.Book>();
        bookRecord.AvailableCount = 300;
        var queryableAvailableBookCount = new List<Entities.Book> { bookRecord }.AsQueryable().BuildMock();
        _booksRepoMock.Setup(repo => repo.GetDataWithLinqExp(It.IsAny<Expression<Func<Entities.Book, bool>>>(), false))
                      .Returns(queryableAvailableBookCount);

        // Act
        var result = await _bookService.CheckIfAnyAvailableBooksAsync(bookRecord.BookId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfAnyAvailableBooksAsync_WhenAvailableBookCountEqualToZero_ShouldReturnFalse()
    {
        // Arrange
        var bookRecord = _fixture.Create<Entities.Book>();
        bookRecord.AvailableCount = 0;
        var queryableAvailableBookCount = new List<Entities.Book> { bookRecord }.AsQueryable().BuildMock();
        _booksRepoMock.Setup(repo => repo.GetDataWithLinqExp(It.IsAny<Expression<Func<Entities.Book, bool>>>(), false))
                      .Returns(queryableAvailableBookCount);

        // Act
        var result = await _bookService.CheckIfAnyAvailableBooksAsync(bookRecord.BookId);

        // Assert
        Assert.False(result);
    }
}
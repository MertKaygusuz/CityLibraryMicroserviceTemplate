using Microsoft.EntityFrameworkCore;
using BookServiceApi.Dtos.Book;
using BookServiceApi.Entities;
using BookServiceApi.Repositories.Book;
using BookServiceApi.Services.Book.Interfaces;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.MapperConfigurations;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Repositories.User;
using BookServiceApi.Services.BookReservationApiService;
using CityLibrary.Shared.SharedModels;
using Microsoft.Extensions.Options;
using UserServiceApi.AppSettings;
using Grpc.Net.Client;
using BookReservationReturn;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core.Interceptors;
using BookServiceApi.Interceptors;
using MassTransit;
using CityLibrary.Shared.SharedModels.QueueModels;

namespace BookServiceApi.Services.Book.Classes
{
    public class BookService : IBookService
    {
        private readonly IBooksRepo _booksRepo;
        private readonly Lazy<IUsersRepo> _usersRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBookReservationRecordApi _bookReservationRecordApi;
        private readonly IOptions<AppSetting> _options;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public BookService(IBooksRepo booksRepo, 
                           IUnitOfWork unitOfWork,
                           Lazy<IUsersRepo> usersRepo,
                           ICustomMapper customMapper,
                           IOptions<AppSetting> options,
                           ISendEndpointProvider sendEndpointProvider,
                           IBookReservationRecordApi bookReservationRecordApi,
                           IHttpContextAccessor httpContextAccessor)
        {
            _booksRepo = booksRepo;
            _usersRepo = usersRepo;
            _unitOfWork = unitOfWork;
            _mapper = customMapper;
            _httpContextAccessor = httpContextAccessor;
            _bookReservationRecordApi = bookReservationRecordApi;
            _options = options;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<int> BookRegisterAsync(RegisterBookDto dto)
        {
            var bookToAdd = _mapper.Map<RegisterBookDto, Books>(dto);
            await _booksRepo.InsertAsync(bookToAdd);
            await _unitOfWork.CommitAsync();
            return bookToAdd.BookId;
        }

        public async Task DeleteBookAsync(DeleteBookDto dto)
        {
            await _booksRepo.DeleteByIdAsync(dto.BookId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> DoesEntityExistAsync(IConvertible Id)
        {
            return await _booksRepo.DoesEntityExistAsync((int)Id);
        }

        public async Task<IEnumerable<Books>> GetAllBooks()
        {
            return await _booksRepo.GetData().OrderBy(x => x.BookTitle).ToListAsync();
        }

        public async Task<int> GetNumberOfAuthorsFromBookTableAsync()
        {
            return await _booksRepo.GetData().Select(x => x.Author).Distinct().CountAsync();
        }

        public async Task<int> GetNumberOfDistinctTitleAsync()
        {
            return await _booksRepo.GetData().Select(x => x.BookTitle).Distinct().CountAsync();
        }

        public async Task UpdateBookInfoAsync(UpdateBookDto dto)
        {
            var existingBook = await _booksRepo.GetByIdAsync(dto.BookId);
            _mapper.MapToExistingObject(dto, existingBook);

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_options.Value.RabbitMqOptions.BookUpdatedSenderUri));
            await sendEndpoint.Send(_mapper.Map<UpdateBookDto, BookUpdated>(dto));

            await _unitOfWork.CommitAsync();
        }

        public async Task AssignBookToUserAsync(AssignBookToUserDto dto)
        {
            var theBook = await _booksRepo.GetByIdAsync(dto.BookId);
            theBook.AvailableCount -= 1;
            theBook.ReservedCount += 1;

            string authHeader = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
            string langHeader = _httpContextAccessor.HttpContext.Request.Headers.AcceptLanguage;

            var theUser = await _usersRepo.Value.GetByIdAsync(dto.UserId);

            var reservationModel = new ActiveBookReservationModel()
            {
                BookId = dto.BookId,
                UserId = dto.UserId,
                DeliveryDateToUser = DateTime.UtcNow,
                Book = _mapper.Map<Books, BookModel>(theBook),
                User = _mapper.Map<Users, UserModel>(theUser)
            };

            await _bookReservationRecordApi.CreateReservation(authHeader, langHeader, reservationModel);

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> CheckIfAnyAvailableBooksAsync(int bookId)
        {
            short availableBookCount = await _booksRepo.GetDataWithLinqExp(x => x.BookId == bookId)
                                                    .Select(x => x.AvailableCount)
                                                    .SingleOrDefaultAsync();

            return availableBookCount > 0;
        }

        public async Task<bool> CheckIfBookExistsAsync(int bookId)
        {
            return await _booksRepo.DoesEntityExistAsync(bookId);
        }

        public async Task UnAssignBookFromUserAsync(AssignBookToUserDto dto)
        {
            var bookRecord = await _booksRepo.GetByIdAsync(dto.BookId);
            bookRecord.ReservedCount -= 1;
            bookRecord.AvailableCount += 1;

            var userRecord = await _usersRepo.Value.GetByIdAsync(dto.UserId);

            using var channel = GrpcChannel.ForAddress(_options.Value.BookReservationGrpcEndPoint);
            var invoker = channel.Intercept(new GrpcHeadersInterceptor(_httpContextAccessor));
            var client = new GrpcBookReservation.GrpcBookReservationClient(invoker);
            var request = new GrpcActiveBookReservationModel
            {
                UserId = dto.UserId,
                User = new GrpcUserModel
                {
                    UserId = dto.UserId,
                    UserName = userRecord.UserName,
                    FullName = userRecord.FullName,
                    BirthDate = Timestamp.FromDateTime(userRecord.BirthDate),
                    Address = userRecord.Address
                },
                BookId = dto.BookId,
                Book = new GrpcBookModel 
                {
                    BookId = dto.BookId,
                    Author = bookRecord.Author,
                    BookTitle = bookRecord.BookTitle,
                    FirstPublishDate = Timestamp.FromDateTime(bookRecord.FirstPublishDate),
                    EditionNumber = bookRecord.EditionNumber,
                    EditionDate = Timestamp.FromDateTime(bookRecord.EditionDate),
                    TitleType = (int) bookRecord.TitleType,
                    CoverType = (int) bookRecord.CoverType
                }
            };

            // grpc call here
            await client.ReturnBookAsync(request);

            await _unitOfWork.CommitAsync();
        }
    }
}

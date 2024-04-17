using AutoFixture;
using BookReservationReportApi.Controllers.Report;
using BookReservationReportApi.Dtos.ReservationReport;
using BookReservationReportApi.Services.ReservationReport.Interfaces;
using Moq;

namespace BookReservationReportApi.UnitTests;

    public class ReportControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IReservationReportService> _reservationReportServiceMock;
        private readonly ReportController _reportController;

        public ReportControllerTests()
        {
            _reservationReportServiceMock = new Mock<IReservationReportService>();
            _reportController = new ReportController(_reservationReportServiceMock.Object);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetActiveBookReservations_WithValidActiveBookReservationsFilterDto_ShouldReturnActiveBookReservations()
        {
            // Arrange
            var dto = _fixture.Create<ActiveBookReservationsFilterDto>();
            var expectedReservations = _fixture.Create<IEnumerable<ActiveBookReservationsResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetAllActiveBookReservationsAsync(dto))
                                         .ReturnsAsync(expectedReservations);

            // Act
            var result = await _reportController.GetActiveBookReservations(dto);

            // Assert
            Assert.Equal(expectedReservations, result);
        }

        [Fact]
        public async Task GetNumberOfBooksReservedPerUsers_ShouldReturnNumberOfBooksReservedPerUsers()
        {
            // Arrange
            var expectedReservations = _fixture.Create<IEnumerable<NumberOfBooksReservedByUsersResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetNumberOfBooksReservedPerUsersAsync())
                                         .ReturnsAsync(expectedReservations);

            // Act
            var result = await _reportController.GetNumberOfBooksReservedPerUsers();

            // Assert
            Assert.Equal(expectedReservations, result);
        }

        [Fact]
        public async Task GetReservationHistoryPerBook_ShouldReturnReservationHistoryBook()
        {
            // Arrange
            var expectedResult = _fixture.Create<IEnumerable<ReservationHistoryBookResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetReservationHistoryPerBookAsync())
                                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _reportController.GetReservationHistoryPerBook();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetReservationHistoryPerUser_ShouldReturnReservationHistoryBook()
        {
            // Arrange
            var expectedResult = _fixture.Create<IEnumerable<ReservationHistoryUserResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetReservationHistoryPerUserAsync())
                                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _reportController.GetReservationHistoryPerUser();

            // Assert
            Assert.Equal(expectedResult, result);
        }


        [Fact]
        public async Task GetReservationHistoryByBook_WithValidReservationHistoryBookDto_ShouldReturnReservationHistoryBook()
        {
            // Arrange
            var dto = _fixture.Create<ReservationHistoryBookDto>();
            var expectedResult = _fixture.Create<IEnumerable<ReservationHistoryBookResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetReservationHistoryByBookAsync(dto))
                                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _reportController.GetReservationHistoryByBook(dto);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetReservationHistoryByUser_WithValidReservationHistoryUserDto_ShouldReturnReservationHistoryUser()
        {
            // Arrange
            var dto = _fixture.Create<ReservationHistoryUserDto>();
            var expectedResult = _fixture.Create<IEnumerable<ReservationHistoryUserResponseDto>>();
            _reservationReportServiceMock.Setup(x => x.GetReservationHistoryByUserAsync(dto))
                                         .ReturnsAsync(expectedResult);

            // Act
            var result = await _reportController.GetReservationHistoryByUser(dto);

            // Assert
            Assert.Equal(expectedResult, result);
        }

    }
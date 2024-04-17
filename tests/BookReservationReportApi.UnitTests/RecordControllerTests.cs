using AutoFixture;
using BookReservationReportApi.Controllers.Record;
using BookReservationReportApi.Services.ReservationReport.Interfaces;
using CityLibrary.Shared.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookReservationReportApi.UnitTests;

public class RecordControllerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IReservationRecordService> _reservationRecordServiceMock;
    private readonly RecordController _recordController;

    public RecordControllerTests()
    {
        _reservationRecordServiceMock = new Mock<IReservationRecordService>();
        _recordController = new RecordController(_reservationRecordServiceMock.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateReservation_ValidDto_ReturnsNoContentResult()
    {
        // Arrange
        var dto = _fixture.Create<ActiveBookReservationModel>();

        // Act
        var result = await _recordController.CreateReservation(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = (NoContentResult)result;
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
    }
}
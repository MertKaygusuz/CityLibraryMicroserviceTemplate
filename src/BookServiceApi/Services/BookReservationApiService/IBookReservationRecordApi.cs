using CityLibrary.Shared.SharedModels;
using Refit;

namespace BookServiceApi.Services.BookReservationApiService
{
    public interface IBookReservationRecordApi
    {
        [Post("/api/Record/CreateReservation")]
        Task CreateReservation([Header("Authorization")] string authJwtToken,
                                [Header("Accept-Language")] string lang,
                                ActiveBookReservationModel request);
    }
}
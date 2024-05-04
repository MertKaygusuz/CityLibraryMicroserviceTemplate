using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace BookServiceApi.IntegrationTests.Mocks
{
    public class BookReservationApiEndPointMocks
    {
        public static void CreateReservationStub(WireMockServer server)
        {
            server.Given(
                Request.Create().WithPath("/api/Record/CreateReservation").UsingPost()
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(204)
                    .WithHeader("Content-Type", "text/plain")
                    // .WithBody()
            );
        }
    }
}
using System.Net;

namespace CityLibrary.Shared.ExceptionHandling
{
    public class CustomInternalServerException : CustomHttpException
    {
        public CustomInternalServerException(string Message)
          : base(Message)
        {

        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.InternalServerError;
    }
}

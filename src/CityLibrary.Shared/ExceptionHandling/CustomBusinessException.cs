using System.Net;


namespace CityLibrary.Shared.ExceptionHandling
{
    public class CustomBusinessException : CustomHttpException
    {
        public CustomBusinessException(string Message)
           : base(Message)
        {

        }

        public override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;
    }
}

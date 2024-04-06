using System.Security.Claims;

namespace CityLibrary.Shared.Extensions.TokenExtensions
{
    public class AccesInfoFromToken
    {
        public static string GetMyUserId()
        {
            if (GlobalHttpContext._contextAccessor?.HttpContext?.User is null) 
                return null;
            return GlobalHttpContext._contextAccessor.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Sid)
                                                                             .Select(x => x.Value)
                                                                             .FirstOrDefault();
        }
    }
}

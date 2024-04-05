using CityLibrary.Shared.ExceptionHandling.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserServiceApi.ActionFilters.Interfaces;


namespace UserServiceApi.ActionFilters.Classes
{
    public class RefreshLoginFilter : IRefreshLoginFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string refreshToken = context.ActionArguments["refreshToken"] as string;

            if (string.IsNullOrEmpty(refreshToken))
            {
                var err = new ErrorDto("Refresh token is empty.");
                context.Result = new ObjectResult(err)
                {
                    StatusCode = err.Status
                };
                return;
            }
            
            await next();
        }
    }
}

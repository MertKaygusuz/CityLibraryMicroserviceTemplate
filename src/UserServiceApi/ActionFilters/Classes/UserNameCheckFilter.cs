using UserServiceApi.ActionFilters.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;
using UserServiceApi.Dtos.User;
using UserServiceApi.Services.User.Interfaces;
using CityLibrary.Shared.ExceptionHandling.Dtos;

namespace UserServiceApi.ActionFilters.Classes
{
    public class UserNameCheckFilter(IUserService userService, IStringLocalizer<ActionFiltersResource> localizer) : IUserNameCheckFilter
    {
        private readonly IUserService _userService = userService;
        private readonly IStringLocalizer<ActionFiltersResource> _localizer = localizer;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            RegistrationDto modelVal = context.ActionArguments["dto"] as RegistrationDto;
            bool doesExist = await _userService.CheckIfExistWithUserNameAsync(modelVal!.UserName);
            if (doesExist)
            {
                var err = new ErrorDto();
                err.Errors.Add(nameof(modelVal.UserName), [string.Format(_localizer["User_Name_Already_Taken"], modelVal.UserName)]);
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

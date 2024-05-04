using BookServiceApi.ActionFilters.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BookServiceApi.Resources;
using Microsoft.Extensions.Localization;
using BookServiceApi.Services.Book.Interfaces;
using BookServiceApi.Dtos.BookReservation;
using CityLibrary.Shared.ExceptionHandling.Dtos;
using BookServiceApi.Services.User.Interfaces;

namespace BookServiceApi.ActionFilters.Classes
{
    public class UnAssigningBookFilter(IBookService bookService, IUserService userService, IStringLocalizer<ActionFiltersResource> localizer) : IUnAssigningBookFilter
    {
        private readonly IBookService _bookService = bookService;
        private readonly IUserService _userService = userService;
        private readonly IStringLocalizer<ActionFiltersResource> _localizer = localizer;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            AssignBookToUserDto modelVal = context.ActionArguments["dto"] as AssignBookToUserDto;

            // following parallel requests are cancelled because AppDbContext's service life time is scoped. (can be used with transient scope in DI container but scoped is better for our cases)
            // var userExistTask = _userService.CheckIfUserExistsAsync(modelVal!.UserId);
            // var bookExistTask = _bookService.CheckIfBookExistsAsync(modelVal.BookId);
            // await Task.WhenAll(userExistTask, bookExistTask); //parallel request to db.
            // bool userExist = userExistTask.Result;
            // bool bookExist = bookExistTask.Result;

            bool userExist = await _userService.CheckIfUserExistsAsync(modelVal!.UserId);
            bool bookExist = await _bookService.CheckIfBookExistsAsync(modelVal.BookId);

            if (!(userExist && bookExist))
            {
                var err = new ErrorDto();
                if (!userExist)
                    err.Errors.Add(nameof(modelVal.UserId), [_localizer["User_Not_Exist"]]);

                if (!bookExist)
                    err.Errors.Add(nameof(modelVal.BookId), [_localizer["Book_Not_Exist"]]);

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

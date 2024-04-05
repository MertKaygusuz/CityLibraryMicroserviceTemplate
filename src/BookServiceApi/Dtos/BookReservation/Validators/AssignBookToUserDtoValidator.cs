using FluentValidation;
using BookServiceApi.Resources;
using Microsoft.Extensions.Localization;

namespace BookServiceApi.Dtos.BookReservation.Validators
{
    public class AssignBookToUserDtoValidator : AbstractValidator<AssignBookToUserDto>
    {
        public AssignBookToUserDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.BookId).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => localizer["Book_Required"])
                .NotEmpty()
                .WithMessage(_ => localizer["Book_Required"]);
            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => localizer["User_Required"])
                .NotEmpty()
                .WithMessage(_ => localizer["User_Required"]);
        }
    }
}

using BookServiceApi.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BookServiceApi.Dtos.Book.Validators
{
    public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
    {
        public UpdateBookDtoValidator(IStringLocalizer<BookValidationResource> localizer)
        {
            Include(new RegisterBookDtoValidator(localizer));
            RuleFor(x => x.BookId).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
        }
    }
}

using FluentValidation;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Dtos.Authentication.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => localizer["User_Name_Required"])
                .NotEmpty()
                .WithMessage(_ => localizer["User_Name_Required"]);
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => localizer["Password_Required"])
                .NotEmpty()
                .WithMessage(_ => localizer["Password_Required"]);
        }
    }
}

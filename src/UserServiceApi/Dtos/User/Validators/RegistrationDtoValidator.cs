using FluentValidation;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Dtos.User.Validators
{
    public class RegistrationDtoValidator : AbstractValidator<RegistrationDto>
    {
        private const byte MinimumUserNameLength = 5;
        private const byte MaximumUserNameLength = 30;
        private const byte MinimumPasswordLength = 8;
        private const short MaximumAddressLength = 300;
        private const byte MaximumFullNameLength = 50;

        public RegistrationDtoValidator(IStringLocalizer<SharedResource> sharedLocalizer,
                                        IStringLocalizer<UserValidationResource> userLocalizer)
        {
            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => sharedLocalizer["User_Name_Required"])
                .NotEmpty()
                .WithMessage(_ => sharedLocalizer["User_Name_Required"])
                .MinimumLength(MinimumUserNameLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Minimum_Length"], MinimumUserNameLength))
                .MaximumLength(MaximumUserNameLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Maximum_Length"], MaximumUserNameLength));
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => sharedLocalizer["Password_Required"])
                .NotEmpty()
                .WithMessage(_ => sharedLocalizer["Password_Required"])
                .MinimumLength(MinimumPasswordLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Minimum_Length"], MinimumPasswordLength));
            RuleFor(x => x.Address).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => userLocalizer["Address_Required"])
                .NotEmpty()
                .WithMessage(_ => userLocalizer["Address_Required"])
                .MaximumLength(MaximumAddressLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Maximum_Length"], MaximumAddressLength));
            RuleFor(x => x.BirthDate).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => userLocalizer["Birth_Date_Required"])
                .NotEmpty()
                .WithMessage(_ => userLocalizer["Birth_Date_Required"]);
            RuleFor(x => x.FullName).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => userLocalizer["Full_Name_Required"])
                .NotEmpty()
                .WithMessage(_ => userLocalizer["Full_Name_Required"])
                .MaximumLength(MaximumFullNameLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Maximum_Length"], MaximumFullNameLength));
        }
    }
}

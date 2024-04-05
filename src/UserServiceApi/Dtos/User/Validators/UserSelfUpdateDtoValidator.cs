using FluentValidation;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Dtos.User.Validators
{
    public class UserSelfUpdateDtoValidator : AbstractValidator<UserSelfUpdateDto>
    {
        private const byte MinimumPasswordLength = 8;
        private const short MaximumAddressLength = 300;
        private const byte MaximumFullNameLength = 50;
        public UserSelfUpdateDtoValidator(IStringLocalizer<SharedResource> sharedLocalizer,
                                            IStringLocalizer<UserValidationResource> memberLocalizer)
        {
            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => sharedLocalizer["Password_Required"])
                .NotEmpty()
                .WithMessage(_ => sharedLocalizer["Password_Required"])
                .MinimumLength(MinimumPasswordLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Minimum_Length"], MinimumPasswordLength));
            RuleFor(x => x.Address).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => memberLocalizer["Address_Required"])
                .NotEmpty()
                .WithMessage(_ => memberLocalizer["Address_Required"])
                .MaximumLength(MaximumAddressLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Maximum_Length"], MaximumAddressLength));
            RuleFor(x => x.BirthDate).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => memberLocalizer["Birth_Date_Required"])
                .NotEmpty()
                .WithMessage(_ => memberLocalizer["Birth_Date_Required"]);
            RuleFor(x => x.FullName).Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(_ => memberLocalizer["Full_Name_Required"])
                .NotEmpty()
                .WithMessage(_ => memberLocalizer["Full_Name_Required"])
                .MaximumLength(MaximumFullNameLength)
                .WithMessage(_ => string.Format(sharedLocalizer["Maximum_Length"], MaximumFullNameLength));
        }
    }
}

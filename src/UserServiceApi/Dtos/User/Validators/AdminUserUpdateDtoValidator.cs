using FluentValidation;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Dtos.User.Validators
{
    public class AdminUserUpdateDtoValidator : AbstractValidator<AdminUserUpdateDto>
    {
        public AdminUserUpdateDtoValidator(IStringLocalizer<SharedResource> sharedLocalizer,
                                           IStringLocalizer<UserValidationResource> userLocalizer)
        {
            Include(new UserSelfUpdateDtoValidator(sharedLocalizer, userLocalizer));
            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop).NotNull().NotEmpty();
        }
    }
}
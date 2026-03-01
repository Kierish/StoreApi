using FluentValidation;
using StoreApi.DTOs;

namespace StoreApi.Validators
{
    public class LoginDataDtoValidator : AbstractValidator<LoginDataDto>
    {
        public LoginDataDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(8).MaximumLength(100);
        }
    }

    public class RegisterDataDtoValidator : AbstractValidator<RegisterDataDto>
    {
        public RegisterDataDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty()
                .MinimumLength(3).MaximumLength(100);

            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .MaximumLength(256);

            RuleFor(x => x.PhoneNumber).NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Password).NotEmpty()
                .MinimumLength(8).MaximumLength(100);
        }
    }
}

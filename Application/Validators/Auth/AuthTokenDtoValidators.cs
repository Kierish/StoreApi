using Application.DTOs.Auth;
using FluentValidation;

namespace StoreApi.Validators.Auth
{
    public class AuthRequestDtoValidator : AbstractValidator<AuthRequestDto>
    {
        public AuthRequestDtoValidator() 
        {
            RuleFor(d => d.RefreshToken)
                .NotEmpty();

            RuleFor(d => d.JwtToken)
                .NotEmpty();
        }
    }
}

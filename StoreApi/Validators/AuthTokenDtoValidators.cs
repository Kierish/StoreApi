using FluentValidation;
using StoreApi.DTOs;

namespace StoreApi.Validators
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

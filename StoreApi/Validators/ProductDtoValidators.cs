using Application.DTOs;
using FluentValidation;

namespace StoreApi.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator(IValidator<PageMetadataCreateDto?> seoValidator)
        {
            RuleFor(d => d.Name).NotEmpty().MinimumLength(3).MaximumLength(100);

            RuleFor(d => d.Price).NotNull().GreaterThan(0);

            RuleFor(d => d.Metadata).SetValidator(seoValidator).When(d => d.Metadata != null);
        }
    }

    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator(IValidator<PageMetadataUpdateDto?> seoValidator)
        {
            RuleFor(d => d.Name).MinimumLength(3).MaximumLength(100).When(d => d.Name != null);

            RuleFor(d => d.Price).GreaterThan(0).When(d => d.Price != null);

            RuleFor(d => d.Metadata).SetValidator(seoValidator).When(d => d.Metadata != null);
        }
    }
}

using FluentValidation;
using StoreApi.DTOs;

namespace StoreApi.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator(IValidator<ProductSeoCreateDto?> seoValidator)
        {
            RuleFor(d => d.Name).NotEmpty()
                .MinimumLength(3).MaximumLength(100);

            RuleFor(d => d.Price).NotNull().GreaterThan(0);
            RuleFor(d => d.CategoryId).GreaterThanOrEqualTo(1);

            RuleFor(d => d.ProductSeo)
                .SetValidator(seoValidator)
                .When(d => d.ProductSeo != null);
        }
    }
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator(IValidator<ProductSeoUpdateDto?> seoValidator)
        {
            RuleFor(d => d.Name).MinimumLength(3).MaximumLength(100)
                .When(d => d.Name != null);

            RuleFor(d => d.Price).GreaterThan(0)
                .When(d => d.Price != null);

            RuleFor(d => d.CategoryId).GreaterThanOrEqualTo(1)
                .When(d => d.CategoryId != null);

            RuleFor(d => d.ProductSeo)
                .SetValidator(seoValidator)
                .When(d => d.ProductSeo != null);
        }
    }
}
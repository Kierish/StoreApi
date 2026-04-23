using Application.DTOs;
using FluentValidation;

namespace StoreApi.Validators
{
    public class ProductSeoCreateDtoValidator : AbstractValidator<PageMetadataCreateDto>
    {
        public ProductSeoCreateDtoValidator()
        {
            RuleFor(d => d.MetaTitle).NotEmpty().MaximumLength(100);

            RuleFor(d => d.OpenGraphImageUrl)
                .NotEmpty()
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("OpenGraphImageUrl must be a valid absolute URL.");
        }
    }

    public class ProductSeoUpdateDtoValidator : AbstractValidator<PageMetadataUpdateDto>
    {
        public ProductSeoUpdateDtoValidator()
        {
            RuleFor(d => d.MetaTitle).MaximumLength(60).When(d => d.MetaTitle != null);

            RuleFor(d => d.OpenGraphImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("OpenGraphImageUrl must be a valid absolute URL.")
                .When(d => d.OpenGraphImageUrl != null);
        }
    }
}

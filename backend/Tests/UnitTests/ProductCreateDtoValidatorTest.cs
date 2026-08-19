using Application.DTOs.Products;
using AutoFixture;
using FluentValidation;
using FluentValidation.TestHelper;
using NSubstitute;
using StoreApi.Validators.Products;

namespace StoreApi.Tests.UnitTests
{
    public class ProductCreateDtoValidatorTest
    {
        private readonly Fixture _fixture = new();
        private readonly IValidator<PageMetadataCreateDto> _mockMetadata = Substitute.For<IValidator<PageMetadataCreateDto>>();
        private readonly ProductCreateDtoValidator _validator;

        public ProductCreateDtoValidatorTest()
        {
            _validator = new ProductCreateDtoValidator(_mockMetadata);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A")]
        [InlineData("Ab")]
        public void ProductCreateDtoValidator_WhenNameIsTooShortOrEmpty_ShouldHaveValidationError(string? badName)
        {
            var dto = _fixture.Build<ProductCreateDto>().With(d => d.Name, badName).Create();


            var result = _validator.TestValidate(dto);


            result.ShouldHaveValidationErrorFor(v => v.Name);
        }

        [Theory]
        [InlineData(101)]
        public void ProductCreateDtoValidator_WhenNameIsTooLong_ShouldHaveValidationError(int stringLength)
        {
            var badName = new string('A', stringLength);
            var dto = _fixture.Build<ProductCreateDto>().With(d => d.Name, badName).Create();


            var result = _validator.TestValidate(dto);


            result.ShouldHaveValidationErrorFor(v => v.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0.0)]  
        [InlineData(-1.0)]
        public void ProductCreateDtoValidator_WhenPriceIsZeroOrLess_ShouldHaveValidationError(double? inputPrice)
        {
            decimal? badPrice = inputPrice.HasValue ? (decimal)inputPrice.Value : null;
            var dto = _fixture.Build<ProductCreateDto>().With(d => d.Price, badPrice).Create();


            var result = _validator.TestValidate(dto);


            result.ShouldHaveValidationErrorFor(v => v.Price);
        }

        [Fact]
        public void ProductCreateDtoValidator_WhenPageMetadataIsNull_ShouldNotHaveValidationError()
        {
            var dto = _fixture.Build<ProductCreateDto>().Without(d => d.PageMetadata).Create();


            var result = _validator.TestValidate(dto);


            result.ShouldNotHaveValidationErrorFor(v => v.PageMetadata);
        }

        [Fact]
        public void ProductCreateDtoValidator_WhenOnlyRequiredFieldsProvided_ShouldNotHaveValidationError()
        {
            var dto = new ProductCreateDto(
                Name: "Name",
                TagNames: null,
                CategoryName: "CategoryName",
                Price: 100,
                PageMetadata: null
                );


            var result = _validator.TestValidate(dto);


            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ProductCreateDtoValidator_WhenAllFieldsAreValid_ShouldNotHaveValidationError()
        {
            var validMetadata = new PageMetadataCreateDto(
                MetaTitle: "Title",
                MetaDescription: "Description",
                Slug: "Slug",
                OpenGraphImageUrl: "https://example.com/images/product-og.png"
            );

            var dto = new ProductCreateDto(
                Name: "Name",
                TagNames: new List<string> { "Tag1", "Tag2" },
                CategoryName: "CategoryName",
                Price: 99.99m,
                PageMetadata: validMetadata
                );


            var result = _validator.TestValidate(dto);


            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

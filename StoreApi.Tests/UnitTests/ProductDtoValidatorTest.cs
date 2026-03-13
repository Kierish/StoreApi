using AutoFixture;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using StoreApi.DTOs;
using StoreApi.Models.Store;
using StoreApi.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.Tests.UnitTests
{
    public class ProductDtoValidatorTest
    {
        [Fact]
        void ProductCreateDtoValidator_WhenNameIsTooShort_ShouldHaveValidationError()
        {
            var fixture = new Fixture();
            var mockVal = new Mock<IValidator<ProductSeoCreateDto?>>();
            var validator = new ProductCreateDtoValidator(mockVal.Object);
            var dto = fixture.Build< ProductCreateDto>()
                .With(d => d.Name, " ").Create();

            var result = validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(v => v.Name);
            result.ShouldNotHaveValidationErrorFor(v => v.Price);
        }
        [Fact]
        void ProductUpdateDtoValidator_WhenImageUrlIsInvalid_ShouldHaveValidationError()
        {
            var fixture = new Fixture();
            var seoValidator = new ProductSeoUpdateDtoValidator();
            var validator = new ProductUpdateDtoValidator(seoValidator!);
            var seo = fixture.Build<ProductSeoUpdateDto>()
                .With(s => s.OpenGraphImageUrl, " ").Create();
            var dto = fixture.Build<ProductUpdateDto>()
                .With(d => d.ProductSeo, seo).Create();

            var result = validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(v => v.ProductSeo!.OpenGraphImageUrl);
        }
    }
}

using Application.DTOs.Products;
using Application.Mappers.Auth;
using Application.Mappers.Products;
using AutoFixture;
using FluentAssertions;

namespace StoreApi.Tests.UnitTests
{
    public class ProductMappersTest
    {
        private readonly Fixture _fixture = new();
        [Fact]
        public void ToEntity_GivenProductCreateDto_ShouldMapToProduct()
        {
            var dto = _fixture.Create<ProductCreateDto>();
            var categoryId = Guid.CreateVersion7();


            var result = dto.ToEntity(categoryId);


            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(dto.Name);
            result.CategoryId.Should().Be(categoryId);
            result.Price.Should().Be(dto.Price);
            result.IsDeleted.Should().Be(false);
        }
    }
}

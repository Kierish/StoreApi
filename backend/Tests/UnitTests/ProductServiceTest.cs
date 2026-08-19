using Application.DTOs.Products;
using Application.Interfaces.Services;
using Application.Repositories;
using Application.Services;
using AutoFixture;
using Domain.Constants;
using Domain.ErrorMessages;
using Domain.Models.Products;
using FluentAssertions;
using NSubstitute;

namespace StoreApi.Tests.UnitTests
{
    public class ProductServiceTest
    {
        private readonly Fixture _fixture = new();
        private readonly IProductRepository _mockRepo = Substitute.For<IProductRepository>();
        private readonly ICacheService _mockCache = Substitute.For<ICacheService>();
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _productService = new ProductService(_mockRepo, _mockCache);
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryNotFound_ShouldReturnFailure()
        {
            var dto = _fixture.Create<ProductCreateDto>();
            _mockRepo.GetCategoryIdAsync(dto.CategoryName).Returns((Guid?)null);


            var result = await _productService.CreateAsync(dto);


            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ProductErrors.CategoryNotFound(dto.CategoryName));

            _mockRepo.DidNotReceive().AddProduct(Arg.Any<Product>());
            await _mockRepo.DidNotReceive().SaveChangesAsync();
            await _mockCache.DidNotReceive().IncrementVersionAsync(Arg.Any<string>());  
        }

        [Fact]
        public async Task CreateAsync_WhenOnlyRequiredDataProvided_ShouldCreateProductAndInvalidateCache()
        {
            var dto = new ProductCreateDto(
                Name: "Name",
                TagNames: null,
                CategoryName: "CategoryName",
                Price: 99.99m,
                PageMetadata: null
                );

            var categoryId = Guid.CreateVersion7();
            _mockRepo.GetCategoryIdAsync(dto.CategoryName).Returns(categoryId);


            var result = await _productService.CreateAsync(dto);


            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();

            await _mockRepo.DidNotReceive().GetTagsContainedInDto(Arg.Any<List<string>>());

            _mockRepo.Received(1).AddProduct(Arg.Is<Product>(p => p.CategoryId == categoryId));
            await _mockRepo.Received(1).SaveChangesAsync();
            await _mockRepo.Received(1).ReferenceCategoryToProduct(Arg.Any<Product>());
            await _mockCache.Received(1).IncrementVersionAsync(CacheKeys.GetProductListVersionKey());
        }

        [Fact]
        public async Task CreateAsync_WhenAllDataProvided_ShouldCreateProductAndInvalidateCache()
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

            var tags = new List<Tag>
            {
                new Tag { Name = "Tag1" },
                new Tag { Name = "Tag2" }
            };

            var categoryId = Guid.CreateVersion7();
            _mockRepo.GetCategoryIdAsync(dto.CategoryName).Returns(categoryId);
            _mockRepo.GetTagsContainedInDto(dto.TagNames!).Returns(tags);


            var result = await _productService.CreateAsync(dto);


            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();

            await _mockRepo.Received(1).GetTagsContainedInDto(Arg.Any<List<string>>());
            _mockRepo.Received(1).AddProduct(Arg.Is<Product>(p =>
                p.CategoryId == categoryId &&
                p.Tags == tags &&         
                p.MetaData != null       
            ));
            await _mockRepo.Received(1).SaveChangesAsync();
            await _mockRepo.Received(1).ReferenceCategoryToProduct(Arg.Any<Product>());
            await _mockCache.Received(1).IncrementVersionAsync(CacheKeys.GetProductListVersionKey());
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductNotFound_ShouldReturnNotFoundFailure()
        {
            var productId = Guid.CreateVersion7();
            _mockRepo.GetProductByIdAsync(productId).Returns((Product?)null);
            _mockCache.GetOrCreateAsync(
                Arg.Any<string>(),
                Arg.Any<Func<Task<ProductReadDto?>>>(),
                Arg.Any<TimeSpan?>(),
                Arg.Any<CancellationToken>()
                ).Returns(callInfo => callInfo.Arg<Func<Task<ProductReadDto?>>>()());


            var result = await _productService.GetByIdAsync(productId);


            result.IsSuccess.Should().BeFalse();
            result.Error.Type.Should().Be(ErrorType.NotFound);

            await _mockRepo.Received(1).GetProductByIdAsync(productId);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductExists_ShouldRemoveProductAndClearCache()
        {
            var productId = Guid.CreateVersion7();

            var existingProduct = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                CategoryId = Guid.CreateVersion7()
            };

            _mockRepo.GetProductByIdAsync(productId).Returns(existingProduct);


            var result = await _productService.DeleteAsync(productId);


            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();

            _mockRepo.Received(1).RemoveProduct(existingProduct);
            await _mockRepo.Received(1).SaveChangesAsync();
            await _mockCache.Received(1).RemoveCacheAsync(CacheKeys.GetProductKey(productId));
            await _mockCache.Received(1).IncrementVersionAsync(CacheKeys.GetProductListVersionKey());
        }

        [Fact]
        public async Task UpdateAsync_WhenTagsDoNotExistInDb_ShouldReturnFailureAndNotSaveChanges()
        {
            var productId = Guid.CreateVersion7();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Product",
                Price = 99.99m,
                CategoryId = Guid.CreateVersion7()
            };

            _mockRepo.GetProductByIdAsync(productId).Returns(existingProduct);

            var updateDto = new ProductUpdateDto(
                Name: null,
                TagNames: new List<string> { "InvalidTag1", "InvalidTag2" },
                CategoryName: null,
                Price: null,
                PageMetadata: null
            );

            _mockRepo.IsTagIdsInDb(Arg.Any<List<string>>()).Returns(false);


            var result = await _productService.UpdateAsync(productId, updateDto);


            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be(ProductErrors.TagsNotFound().Code);

            await _mockRepo.DidNotReceive().GetTagsContainedInDto(Arg.Any<List<string>>());
            await _mockRepo.DidNotReceive().SaveChangesAsync();
            await _mockCache.DidNotReceive().RemoveCacheAsync(Arg.Any<string>());
            await _mockCache.DidNotReceive().IncrementVersionAsync(Arg.Any<string>());
        }
    }
}

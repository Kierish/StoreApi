using AutoFixture;
using Moq;
using StoreApi.Common.Constants;
using StoreApi.DTOs.Products;
using StoreApi.Models.Store;
using StoreApi.Repositories.Products;
using StoreApi.Services.Products; 

namespace StoreApi.Tests.UnitTests
{
    public class ProductServiceTest
    {
        [Fact]
        public async Task GetByIdAsync_WhenRepositoryReturnNull_ReturnsFailureResult() 
        {
            var mockRepo = new Mock<IProductRepository>();
            var myGuid = Guid.NewGuid();
            mockRepo.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);

            var sut = new ProductService(mockRepo.Object);

            var result = await sut.GetByIdAsync(myGuid);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.Error.Type);

            mockRepo.Verify(s => s.GetProductByIdAsync(myGuid), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductExists_ReturnsSuccessResult() 
        {
            var fixture = new Fixture();
            var mockRepo = new Mock<IProductRepository>();
            var targetId = Guid.NewGuid();

            var existingProduct = fixture.Build<Product>()
                .With(p => p.Id, targetId)
                .With(p => p.Price, 10.0m)
                .Without(p => p.Tags)
                .Without(p => p.Category)
                .Without(p => p.ProductSeo).Create();

            mockRepo.Setup(s => s.GetProductByIdAsync(targetId))
                .ReturnsAsync(existingProduct);

            var sut = new ProductService(mockRepo.Object);

            var result = await sut.DeleteAsync(targetId);

            Assert.True(result.IsSuccess);

            mockRepo.Verify(s => s.RemoveProduct(existingProduct), Times.Once);
            mockRepo.Verify(s => s.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenTagsNotContainedInDb_ReturnsFailureResult() 
        {
            var fixture = new Fixture();
            var mockRepo = new Mock<IProductRepository>();
            var myGuid = Guid.NewGuid();

            var existingProduct = fixture.Build<Product>()
                .With(p => p.Id, myGuid)
                .With(p => p.Price, 10.0m)
                .With(p => p.Tags, new List<Tag> { new Tag { Name = "Name1" }, new Tag { Name = "Name2" } })
                .Without(p => p.Category)
                .Without(p => p.ProductSeo).Create();

            mockRepo.Setup(s => s.GetProductByIdAsync(myGuid))
                .ReturnsAsync(existingProduct);

            var changedProductDto = new ProductUpdateDto(null,
                TagNames: new List<string>() { "Name2", "Name3" }, null, null, null);

            mockRepo.Setup(s => s.GetTagsContainedInDto(changedProductDto.TagNames!))
                .ReturnsAsync(new List<Tag>() { new Tag { Name = "Name2" } });

            mockRepo.Setup(s => s.IsTagIdsInDb(changedProductDto.TagNames!))
                .ReturnsAsync(false);

            var sut = new ProductService(mockRepo.Object);

            var result = await sut.UpdateAsync(myGuid, changedProductDto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Tags.NotFound", result.Error.Code);

            mockRepo.Verify(s => s.SaveChangesAsync(), Times.Never);
        }
    }
}

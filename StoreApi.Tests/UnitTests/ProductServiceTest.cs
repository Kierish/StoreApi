using AutoFixture;
using Moq;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces.Repositories;
using StoreApi.Interfaces.Services;
using StoreApi.Models.Store;
using StoreApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.Tests.UnitTests
{
    public class ProductServiceTest
    {
        [Fact]
        public async Task GetByIdAsync_WhenRepositoryReturnNull_ThrowNotFoundException()
        {
            var mockRepo = new Mock<IProductRepository>();
            var myGuid = Guid.NewGuid();
            mockRepo.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);

            var sut = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => sut.GetByIdAsync(myGuid));

            mockRepo.Verify(s => s.GetProductByIdAsync(myGuid), Times.Once);
            mockRepo.Verify(s => s.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenRepositoryReturnNull_ThrowNotFoundException()
        {
            var fixture = new Fixture();
            var mockRepo = new Mock<IProductRepository>();
            var tergetId = Guid.NewGuid();
            var existingProduct = fixture.Build<Product>()
                .With(p => p.Id, tergetId)
                .With(p => p.Price, 10.0m)
                .Without(p => p.Tags)
                .Without(p => p.Category)
                .Without(p => p.ProductSeo).Create();

            mockRepo.Setup(s => s.GetProductByIdAsync(tergetId))
                .ReturnsAsync(existingProduct);
            var sut = new ProductService(mockRepo.Object);

            await sut.DeleteAsync(tergetId);

            mockRepo.Verify(s => s.RemoveProduct(existingProduct), Times.Once);
            mockRepo.Verify(s => s.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenTagsNotContainedInDb_ThrowNotFoundException()
        {
            var fixture = new Fixture();
            var mockRepo = new Mock<IProductRepository>();
            var myGuid = Guid.NewGuid();
            var existingProduct = fixture.Build<Product>()
                .With(p => p.Id, myGuid)
                .With(p => p.Price, 10.0m)
                .With(p => p.Tags, new List<Tag>()
                {
                    new Tag { Name = "Name1" },
                    new Tag { Name = "Name2" }
                })
                .Without(p => p.Category)
                .Without(p => p.ProductSeo).Create();

            mockRepo.Setup(s => s.GetProductByIdAsync(myGuid))
                .ReturnsAsync(existingProduct);

            var changedProductDto = new ProductUpdateDto(null,
                TagNames: new List<string>() { "Name2", "Name3" }, null, null, null);

            mockRepo.Setup(s => s.GetTagsContainedInDto(changedProductDto.TagNames!))
                .ReturnsAsync(new List<Tag>()
                {
                    new Tag { Name = "Name2" }
                });

            var sut = new ProductService(mockRepo.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => sut.UpdateAsync(myGuid, changedProductDto));
            mockRepo.Verify(s => s.SaveChangesAsync(), Times.Never);
        }
    }
}

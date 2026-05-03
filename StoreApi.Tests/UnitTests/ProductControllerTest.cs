using Application.DTOs.Products;
using Application.Interfaces.Services;
using AutoFixture;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StoreApi.Controllers;

namespace StoreApi.Tests.UnitTests
{
    public class ProductControllerTest
    {
        [Fact]
        public async Task AddProduct_WhenSuccessful_ReturnsCreatedAtActionResult()
        {
            var fixture = new Fixture();
            var inputDto = fixture.Create<ProductCreateDto>();
            var returnedDto = fixture.Create<ProductReadDto>();
            var mockService = new Mock<IProductService>();

            mockService.Setup(s => s.CreateAsync(inputDto))
                .ReturnsAsync(Result<ProductReadDto>.Success(returnedDto));

            var mockLogger = new Mock<ILogger<ProductController>>();

            var controller = new ProductController(mockService.Object, mockLogger.Object);

            var actionResult = await controller.AddProduct(inputDto);

            var result = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(returnedDto, result.Value);
        }
    }
}

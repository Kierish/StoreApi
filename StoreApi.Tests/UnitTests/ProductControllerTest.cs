using AutoFixture;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StoreApi.Common.Primitives;
using StoreApi.Controllers;
using StoreApi.DTOs.Products;
using StoreApi.Services;
using StoreApi.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using AutoFixture;
using StoreApi.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using StoreApi.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using StoreApi.Services;
using StoreApi.Controllers;
using Microsoft.AspNetCore.Mvc;

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
                .ReturnsAsync(returnedDto);

            var controller = new ProductController(mockService.Object);

            var actionResult = await controller.AddProduct(inputDto);

            var result = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal(returnedDto, result.Value);
        }
    }
}

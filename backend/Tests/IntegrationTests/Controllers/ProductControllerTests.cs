using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Products;
using Domain.Models.Products;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace IntegrationTests
{
    public class ProductControllerTests : BaseIntegrationTest
    {
        public ProductControllerTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetProductById_WhenProductExists_ShouldReturn200OkWithData()
        {
            var category = new Category { Id = Guid.CreateVersion7(), Name = "Category" };
            var product = new Product
            {
                Id = Guid.CreateVersion7(),
                Name = "Product",
                Price = 99.99m,
                CategoryId = category.Id
            };

            DbContext.Categories.Add(category);
            DbContext.Products.Add(product);
            await DbContext.SaveChangesAsync();


            var response = await Client.GetAsync($"/api/product/{product.Id}");


            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var returnedProduct = await response.Content.ReadFromJsonAsync<ProductReadDto>();
            returnedProduct.Should().NotBeNull();

            returnedProduct.Id.Should().Be(product.Id);
            returnedProduct.Name.Should().Be(product.Name);
            returnedProduct.Price.Should().Be(product.Price);
        }
    }
}

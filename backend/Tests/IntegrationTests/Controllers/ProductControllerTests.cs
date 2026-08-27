using Application.DTOs.Products;
using Domain.Constants;
using Domain.Models.Products;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

        [Fact]
        public async Task GetProductById_WhenProductDoesNotExist_ShouldReturn404NotFound()
        {
            var nonExistentId = Guid.CreateVersion7();


            var response = await Client.GetAsync($"/api/product/{nonExistentId}");
             

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(UserRoles.Admin)]
        [InlineData(UserRoles.Employee)]
        public async Task AddProduct_WhenValidPayload_ShouldReturn201CreatedWithData(string role)
        {
            Authenticate(role);
            var category = new Category
            {
                Id = Guid.CreateVersion7(),
                Name = "Category"
            };
            DbContext.Add(category);
            await DbContext.SaveChangesAsync();

            var dto = new ProductCreateDto(
                Name: "Name",
                TagNames: null,
                CategoryName: "Category",
                Price: 99.99m,
                PageMetadata: null
                );

                
            var response = await Client.PostAsJsonAsync("/api/product", dto);


            response.StatusCode.Should().Be(HttpStatusCode.Created);

            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location.ToString().Should().Contain("/api/Product/");

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductReadDto>();
            createdProduct.Should().NotBeNull();
            createdProduct.Id.Should().NotBeEmpty();
            createdProduct.Name.Should().Be(dto.Name);
            createdProduct.Price.Should().Be(dto.Price);
            createdProduct.CategoryId.Should().Be(category.Id);

            var productInDb = await DbContext.Products.FindAsync(createdProduct.Id);
            productInDb.Should().NotBeNull();
            productInDb!.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task AddProduct_WhenUserIsCustomer_ShouldReturn403Forbidden()
        {
            Authenticate(UserRoles.Customer);
            var dto = new ProductCreateDto(
                Name: "Name",
                TagNames: null,
                CategoryName: "Category",
                Price: 99.99m,
                PageMetadata: null
                );


            var response = await Client.PostAsJsonAsync("/api/product", dto);


            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddProduct_WhenPayloadIsInvalid_ShouldReturn400BadRequest()
        {
            Authenticate(UserRoles.Admin);
            var invalidDto = new ProductCreateDto(
                Name: "",
                TagNames: null,
                CategoryName: "Category",
                Price: 99.99m,
                PageMetadata: null
                );
             

            var response = await Client.PostAsJsonAsync("/api/product", invalidDto);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

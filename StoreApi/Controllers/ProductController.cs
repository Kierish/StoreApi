using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Common.Constants;
using StoreApi.Common.Pagination;
using StoreApi.DTOs.Products;
using StoreApi.Services.Products;
using System.Text.Json;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ApiControllerBase<ProductController>
    {
        private readonly IProductService _service;
        public ProductController(IProductService service,
            ILogger<ProductController> logger) : base(logger) 
        {
            _service = service;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts([FromQuery] PageParameters pageParameters)
        {
            var pageProducts = await _service.GetAllAsync(pageParameters);

            var metadata = new
            {
                pageProducts.TotalCount,
                pageProducts.PageSize,
                pageProducts.Page,
                pageProducts.HasNextPage,
                pageProducts.HasPreviousPage
            };

            Response.Headers["X-Pagination"] = JsonSerializer.Serialize(metadata);

            return Ok(pageProducts.Items);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        public async Task<ActionResult<ProductReadDto>> GetProductById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        public async Task<ActionResult<ProductReadDto>> AddProduct([FromBody] ProductCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("Product {ProductId} added successfully", result.Data!.Id);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("ProductId {ProductId} was successfully changed with data: {@ProductDto}", id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            _logger.LogInformation("ProductId {ProductId} was successfully deleted", id);

            return NoContent();
        }
    }
}

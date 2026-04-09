using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Constants;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Filters;
using StoreApi.Interfaces.Services;
using System.Text.Json;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ProductReadDto>> GetProducts([FromQuery] PageParameters pageParameters)
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
            var product = await _service.GetByIdAsync(id);

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ServiceFilter(typeof(ValidationFilter<ProductCreateDto>))]
        public async Task<ActionResult<ProductReadDto>> AddProduct([FromBody] ProductCreateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ServiceFilter(typeof(ValidationFilter<ProductUpdateDto>))]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            await _service.UpdateAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}

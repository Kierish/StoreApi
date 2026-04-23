using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Filters;

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
        public async Task<ActionResult<List<ProductReadDto>>> GetProducts()
        {
            return Ok(await _service.GetAllAsync());
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
        public async Task<ActionResult<ProductReadDto>> AddProduct(ProductCreateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ServiceFilter(typeof(ValidationFilter<ProductUpdateDto>))]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateDto dto)
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

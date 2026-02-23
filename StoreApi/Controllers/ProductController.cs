using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Services;
using StoreApi.Exceptions;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<ProductReadDto>> GetProductById(int id)
        {
            var product = await _service.GetByIdAsync(id);

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> AddProduct(ProductCreateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            var result = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            await _service.UpdateAsync(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _service.DeleteAsync(id);
   
            return NoContent();
        }
    }
}

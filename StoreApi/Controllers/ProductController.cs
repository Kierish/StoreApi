using Microsoft.AspNetCore.Mvc;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Services;
using StoreApi.Exceptions;

namespace StoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }
        [HttpGet]
        public ActionResult<List<ProductReadDto>> GetProducts()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<ProductReadDto> GetProductById(int id)
        {
            var product = _service.GetById(id);

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<ProductReadDto> AddProduct(ProductCreateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            var result = _service.Create(dto);

            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductUpdateDto dto)
        {
            if (dto is null)
                throw new BadRequestException("Bad data.");

            _service.Update(id, dto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _service.Delete(id);
   
            return NoContent();
        }
    }
}

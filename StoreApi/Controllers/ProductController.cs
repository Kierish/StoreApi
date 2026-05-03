using Application.DTOs.Products;
using Application.Interfaces.Services;
using Application.Pagination;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="pageParameters">Pagination parameters including Page number and PageSize.</param>
        /// <returns>A paginated list of product read data transfer objects.</returns>
        /// <response code="200">Successfully retrieved the products. Pagination metadata is included in the 'X-Pagination' header.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ProductReadDto>), StatusCodes.Status200OK)]
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

        /// <summary>
        /// Retrieves details of a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <returns>The product details.</returns>
        /// <response code="200">Successfully retrieved the product.</response>
        /// <response code="404">The requested product was not found.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDto>> GetProductById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return HandleFailure(result);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The data required to create a new product, including category, tags, and SEO details.</param>
        /// <returns>The newly created product.</returns>
        /// <response code="201">The product was successfully created.</response>
        [HttpPost]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="dto">The updated product data.</param>
        /// <response code="204">The product was successfully updated.</response>
        /// <response code="404">The requested product, category, or tags were not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

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

        /// <summary>
        /// Deletes an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <response code="204">The product was successfully deleted.</response>
        /// <response code="404">The requested product was not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Customer + "," + UserRoles.Employee)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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

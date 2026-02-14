using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Exceptions;

namespace StoreApi.Services
{
    public class ProductService : IProductService
    {
        private static List<Product> _products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1000 },
                new Product { Id = 2, Name = "Mouse", Category = "Accessories", Price = 50 }
            };
        public List<ProductReadDto> GetAll()
        {
            return _products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price
            }).ToList();
        }
        public ProductReadDto? GetById(int id)
        {
            var product = _products.FirstOrDefault(x => x.Id == id);

            if (product == null) 
                throw new NotFoundException($"User with ID {id} was not found.");

            return new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };
        }
        public ProductReadDto Create(ProductCreateDto dto)
        {

            var newProduct = new Product
            {
                Id = (_products.Max(x => x.Id) + 1),
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price
            };
            _products.Add(newProduct);

            return new ProductReadDto
            {
                Id = newProduct.Id,
                Name = newProduct.Name,
                Category = newProduct.Category,
                Price = newProduct.Price
            };
        }
        public void Update(int id, ProductUpdateDto dto)
        {
            var index = _products.FindIndex(pr => pr.Id == id);

            if (index == -1)
                throw new NotFoundException($"User with ID {id} was not found.");

            var product = _products[index];

            product.Name = dto.Name ?? product.Name;
            product.Category = dto.Category ?? product.Category;
            product.Price = dto.Price.HasValue ? dto.Price.Value : product.Price;
        }
        public void Delete(int id)
        {
            var realProduct = _products.FirstOrDefault(p => p.Id == id);

            if (realProduct == null)
                throw new NotFoundException($"User with ID {id} was not found.");

            _products.Remove(realProduct);
        }
    }
}

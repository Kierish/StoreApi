using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Models;
using StoreApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace StoreApi.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }
        public List<ProductReadDto> GetAll()
        {
            var products = _context.Products
                .Include(pr => pr.ProductSeo)
                .Include(pr => pr.Category)
                .Include(pr => pr.Tags)
                .ToList();

            return products.Select(p => new ProductReadDto(
                p.Id,
                p.Name,
                p.Tags?.Select(t => t.Name).ToList(),
                p.CategoryId,
                p.Category?.Name,
                p.Price,
                p.ProductSeo is not null ? new ProductSeoReadDto(
                    p.ProductSeo!.MetaTitle,
                    p.ProductSeo.MetaDescription,
                    p.ProductSeo.Slug,
                    p.ProductSeo.OpenGraphImageUrl
                ) : null
            )).ToList();
        }
        public ProductReadDto? GetById(int id)
        {
            var product = _context.Products
                .Include(pr => pr.Category)
                .Include(pr => pr.Tags)
                .Include(pr => pr.ProductSeo)
                .FirstOrDefault(x => x.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            return new ProductReadDto(
                product.Id,
                product.Name,
                product.Tags?.Select(t => t.Name).ToList(),
                product.CategoryId,
                product.Category?.Name,
                product.Price,
                product.ProductSeo is not null ? new ProductSeoReadDto(
                    product.ProductSeo!.MetaTitle,
                    product.ProductSeo.MetaDescription,
                    product.ProductSeo.Slug,
                    product.ProductSeo.OpenGraphImageUrl
                ) : null
            );
        }
        public ProductReadDto Create(ProductCreateDto dto)
        {
            var newProduct = new Product
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                Price = dto.Price!.Value
            };

            var tags = dto.TagIds is not null ? _context.Tags
                .Where(t => dto.TagIds.Contains(t.Id))
                .ToList() : null;

            newProduct.Tags = tags;

            newProduct.ProductSeo = dto.ProductSeo is not null ? new ProductSeo
            {
                Product = newProduct,
                MetaTitle = dto.ProductSeo.MetaTitle,

                MetaDescription = dto.ProductSeo.MetaDescription,
                Slug = dto.ProductSeo.Slug,
                OpenGraphImageUrl = dto.ProductSeo.OpenGraphImageUrl
            } : null;

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            var CategoryName = _context.Categories
                .Where(c => c.Id == newProduct.CategoryId)
                .Select(c => c.Name)
                .FirstOrDefault();

            return new ProductReadDto(
                newProduct.Id,
                newProduct.Name,
                tags?.Select(t => t.Name).ToList(),
                newProduct.CategoryId,
                CategoryName,
                newProduct.Price,
                new ProductSeoReadDto(
                    newProduct.ProductSeo!.MetaTitle,
                    newProduct.ProductSeo.MetaDescription,
                    newProduct.ProductSeo.Slug,
                    newProduct.ProductSeo.OpenGraphImageUrl
                )
            );
        }
        public void Update(int id, ProductUpdateDto dto)
        {
            var product = _context.Products
                .Include(pr => pr.Tags)
                .Include(pr => pr.ProductSeo)
                .FirstOrDefault(pr => pr.Id == id);

            if (product is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            product.Name = dto.Name ?? product.Name;
            product.CategoryId = dto.CategoryId ?? product.CategoryId;
            product.Price = dto.Price.HasValue ? dto.Price.Value : product.Price;

            if (dto.TagIds is not null)
            {
                var newTags = _context.Tags
                    .Where(t => dto.TagIds.Contains(t.Id))
                    .ToList();

                product.Tags?.Clear();

                product.Tags?.AddRange(newTags);
            }

            var seoDto = dto.ProductSeo;
            var existingSeo = product.ProductSeo;
            if (dto.ProductSeo is not null)
            {
                if (product.ProductSeo is not null)
                {
                    existingSeo.MetaTitle = seoDto.MetaTitle;
                    existingSeo.MetaDescription = seoDto.MetaDescription;
                    existingSeo.Slug = seoDto.Slug;
                    existingSeo.OpenGraphImageUrl = seoDto.OpenGraphImageUrl;
                }
                else
                {
                    product.ProductSeo = new ProductSeo
                    {
                        MetaTitle = seoDto.MetaTitle,
                        MetaDescription = seoDto.MetaDescription,
                        Slug = seoDto.Slug,
                        OpenGraphImageUrl = seoDto.OpenGraphImageUrl
                    };
                }

            }

            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var realProduct = _context.Products.FirstOrDefault(p => p.Id == id);

            if (realProduct is null)
                throw new NotFoundException($"Product with ID {id} was not found.");

            _context.Products.Remove(realProduct);
            _context.SaveChanges();
        }
    }
}

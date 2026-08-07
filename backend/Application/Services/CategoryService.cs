using Application.DTOs.Products;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappers.Products;

using Domain.Constants;
using Domain.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly ICacheService _cache;

        public CategoryService(ICategoryRepository repo, 
            ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync()
        {
            string cacheKey = CacheKeys.GetCategoryListKey();
            var cachedDtos = await _cache.GetOrCreateAsync<IEnumerable<CategoryReadDto>>(
                cacheKey,
                async () =>
                {
                    var categories = await _repo.GetAllCategoriesAsync();
                    var dtos = categories.Select(c => c.ToReadDto());

                    return dtos;
                },
                TimeSpan.FromMinutes(10)
            );

            return Result<IEnumerable<CategoryReadDto>>.Success(cachedDtos);
        }
    }
}

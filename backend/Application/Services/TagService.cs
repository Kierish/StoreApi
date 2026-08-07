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
    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        private readonly ICacheService _cache;
        
        public TagService(ITagRepository repo,
            ICacheService cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<Result<IEnumerable<TagReadDto>>> GetAllTagsAsync()
        {
            string cacheKey = CacheKeys.GetTagsListKey();
            var cachedDtos = await _cache.GetOrCreateAsync<IEnumerable<TagReadDto>>(
                cacheKey,
                async () =>
                {
                    var tags = await _repo.GetAllTagsAsync();
                    var dtos = tags.Select(t => t.ToReadDto());

                    return dtos;
                },
                TimeSpan.FromMinutes(10)
            );

            return Result<IEnumerable<TagReadDto>>.Success(cachedDtos);
        }
    }
}

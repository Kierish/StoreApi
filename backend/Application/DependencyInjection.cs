using Application.Interfaces.Services;
using Application.Services;
using Application.Services.CacheService;
using Application.Validators.Pagination;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICommentService, CommentService>();

            // Cache Services
            services.AddSingleton<CacheService>();
            services.AddSingleton<ICacheService>(provider =>
                new ResilientCacheService(
                    provider.GetRequiredService<CacheService>(),
                    provider.GetRequiredService<ILogger<ResilientCacheService>>()
                ));

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<PageParametersValidator>();

            return services;
        }
    }
}

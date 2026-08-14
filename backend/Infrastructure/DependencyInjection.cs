using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Repositories;
using Infrastructure.Auth;
using Infrastructure.Auth.Options;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // Redis    
            var redisConfiguration = configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
                options.InstanceName = "StoreApi_v1_";
            });

            // Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            // Auth & Password Hashing
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            // JWT Setup
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtOptions>();
            if (
                jwtSettings is null
                || string.IsNullOrEmpty(jwtSettings.Issuer)
                || string.IsNullOrEmpty(jwtSettings.Audience)
                || string.IsNullOrEmpty(jwtSettings.SecretKey)
            )
            {
                throw new InvalidOperationException("JwtSettings missed something from configuration.");
            }

            services.Configure<JwtOptions>(configuration.GetSection("JwtSettings"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                        ClockSkew = TimeSpan.Zero,
                    };
                });

            return services;
        }
    }
}

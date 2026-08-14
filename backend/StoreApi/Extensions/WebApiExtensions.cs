using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using StoreApi.Infrastructure.Exceptions;
using StoreApi.Infrastructure.Filters;
using StoreApi.Infrastructure.Swagger;
using System.Globalization;
using System.Reflection;

namespace StoreApi.Extensions
{
    public static class WebApiExtensions
    {
        public static IServiceCollection AddWebServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Culture Settings
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Exception Handling & Health Checks
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddHealthChecks();

            // CORS
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowReactApp",
                    policy =>
                    {
                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("X-Pagination");
                    }
                );
            });

            // Controllers & Validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalValidationFilter>();
            });

            // Swagger
            services.AddFluentValidationRulesToSwagger();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Store Management API",
                        Version = "v1",
                        Description = "A production-grade REST API for store management.",
                    }
                );

                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token.",
                    }
                );

                options.OperationFilter<AuthOperationFilter>();
                options.OperationFilter<GlobalResponsesOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StoreApi.Infrastructure.Swagger
{
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo
                                    .DeclaringType!
                                    .GetCustomAttributes(true)
                                    .OfType<AuthorizeAttribute>()
                                    .Any()
                                ||
                                context.MethodInfo
                                    .GetCustomAttributes(true)
                                    .OfType<AuthorizeAttribute>()
                                    .Any();

            var hasAllowAnonymous = context.MethodInfo
                                        .DeclaringType!
                                        .GetCustomAttributes(true)
                                        .OfType<AllowAnonymousAttribute>()
                                        .Any()
                                    ||
                                    context.MethodInfo
                                        .GetCustomAttributes(true)
                                        .OfType<AllowAnonymousAttribute>()
                                        .Any();

            if (!hasAuthorize || hasAllowAnonymous)
                return;

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }
    }
}
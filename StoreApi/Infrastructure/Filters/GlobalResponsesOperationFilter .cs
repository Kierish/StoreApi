using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StoreApi.Infrastructure.Swagger
{
    public class GlobalResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var problemDetailsSchema = context.SchemaGenerator
                .GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);

            OpenApiMediaType MediaType() => new OpenApiMediaType { Schema = problemDetailsSchema };

            var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            var allowAnonymous = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ||
                                 context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (hasAuthorize && !allowAnonymous)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized - Valid JWT required.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = MediaType() }
                });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden - User lacks required roles.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = MediaType() }
                });
            }

            var hasFromBody = context.MethodInfo.GetParameters()
                .Any(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(FromBodyAttribute)));

            if (hasFromBody)
            {
                operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Bad Request - Validation failed.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = MediaType() }
                });
            }
        }
    }
}
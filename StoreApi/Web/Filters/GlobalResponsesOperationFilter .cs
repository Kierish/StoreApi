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
            var standardSchema = context.SchemaGenerator
                .GenerateSchema(typeof(ProblemDetails), context.SchemaRepository);

            var validationSchema = context.SchemaGenerator
                .GenerateSchema(typeof(HttpValidationProblemDetails), context.SchemaRepository);

            var standardContent = new OpenApiMediaType { Schema = standardSchema };
            var validationContent = new OpenApiMediaType { Schema = validationSchema };

            var hasAuthorize = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            var allowAnonymous = context.MethodInfo.DeclaringType!.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ||
                                 context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (hasAuthorize && !allowAnonymous)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse
                {
                    Description = "Unauthorized - Valid JWT required.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = standardContent }
                });
                operation.Responses.TryAdd("403", new OpenApiResponse
                {
                    Description = "Forbidden - User lacks required roles.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = standardContent }
                });
            }

            var hasFromBody = context.MethodInfo.GetParameters()
                .Any(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(FromBodyAttribute)));

            if (hasFromBody)
            {
                operation.Responses.TryAdd("400", new OpenApiResponse
                {
                    Description = "Bad Request - Validation failed.",
                    Content = new Dictionary<string, OpenApiMediaType> { ["application/json"] = validationContent }
                });
            }
        }
    }
}
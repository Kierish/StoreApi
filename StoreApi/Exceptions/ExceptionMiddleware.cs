using StoreApi.Models;
using System.Net;
using System.Text.Json;

namespace StoreApi.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            switch (ex)
            {
                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;

                case BadRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
            }

            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Details = ex.StackTrace
            };

            var json = JsonSerializer.Serialize(response);  

            await context.Response.WriteAsync(json);
        }
    }
}

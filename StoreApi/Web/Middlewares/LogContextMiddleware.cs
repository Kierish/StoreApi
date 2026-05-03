using Serilog.Context;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace StoreApi.Infrastructure.Middlewares
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public LogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            using (LogContext.PushProperty("UserId", userId ?? "anonymous"))
            {
                await _next(context);
            }
        }
    }
}

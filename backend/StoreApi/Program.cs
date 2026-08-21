using Application;
using Infrastructure;
using Serilog;
using StoreApi.Extensions;
using StoreApi.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Host.UseSerilog(
    (context, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    }
);

// Register Layer Dependencies
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

// Configure HTTP Request Pipeline
var app = builder.Build();

app.MapHealthChecks("/health");

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<LogContextMiddleware>();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();

public partial class Program { }

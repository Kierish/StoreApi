using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Data;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
using StoreApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IAccountSevice, AccountService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var myKey = builder.Configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret Key is missing from configuration.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience =  builder.Configuration["JwtSettings:Audience"],

            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myKey))
        };

    });

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        logger.LogInformation("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.UseAuthentication();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

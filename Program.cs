using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Repositories;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TMSDbContext>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("TMS-Database"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("TMS-Database"))
));

builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleTransformer>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<IClientRepo, ClientRepo>();

// CORS for Angular front-end (adjust origin as needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Keycloak / JWT authentication
var keycloakSection = builder.Configuration.GetSection("Keycloak");
var authority = keycloakSection.GetValue<string>("Authority");
var clientId = keycloakSection.GetValue<string>("ClientId");
var requireHttps = keycloakSection.GetValue<bool?>("RequireHttpsMetadata") ?? true;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = requireHttps;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = clientId,
            ValidateIssuer = true,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

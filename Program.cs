using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Interfaces;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Data.Repositories;
using Transport_Management_Systems_Portal_Order_Service_REST_API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TMSDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("TMS-Database"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("TMS-Database"))));

builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleTransformer>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<IClientRepo, ClientRepo>();
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

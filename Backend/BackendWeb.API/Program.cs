// ---------
// Libraries
// ---------
using Microsoft.EntityFrameworkCore;
using BackendWeb.API.Application.Interfaces;
using BackendWeb.API.Application.Services;
using BackendWeb.API.Infrastructure.Data;

// ---------
// Builder
// ---------
var builder = WebApplication.CreateBuilder(args);

// ---------
// Dependency Injections
// ---------

// Base de datos
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    ));

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---------
// App
// ---------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------
// APIs
// ---------
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ---------
// Execution 
// ---------
app.Run();

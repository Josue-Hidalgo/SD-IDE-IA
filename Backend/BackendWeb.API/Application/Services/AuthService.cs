using Microsoft.EntityFrameworkCore;
using BackendWeb.API.Application.DTOs.Auth;
using BackendWeb.API.Application.Interfaces;
using BackendWeb.API.Domain.Entities;
using BackendWeb.API.Infrastructure.Data;

namespace BackendWeb.API.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        // Verificar si el email ya existe
        var exists = await _db.Users.AnyAsync(u => u.EmailUser == dto.Email);
        if (exists) return false;

        // Crear el usuario con contraseña hasheada
        var user = new User
        {
            EmailUser = dto.Email,
            PasswordUser = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            NameUser = dto.Name,
            LastnameUser = dto.Lastname
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Crear el estudiante asociado
        var student = new Student { IdUser = user.IdUser };
        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.EmailUser == dto.Email);

        if (user is null) return false;

        return BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordUser);
    }
}
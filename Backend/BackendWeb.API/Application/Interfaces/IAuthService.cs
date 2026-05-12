using BackendWeb.API.Application.DTOs.Auth;

namespace BackendWeb.API.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto);
    Task<bool> LoginAsync(LoginDto dto);
}
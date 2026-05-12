namespace BackendWeb.API.Application.DTOs.Auth;

public record RegisterDto(
    string Name,
    string Lastname,
    string Email,
    string Password
);
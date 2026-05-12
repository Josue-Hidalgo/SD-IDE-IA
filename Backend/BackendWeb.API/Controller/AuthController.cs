using Microsoft.AspNetCore.Mvc;
using BackendWeb.API.Application.DTOs.Auth;
using BackendWeb.API.Application.Interfaces;

namespace BackendWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var success = await _authService.RegisterAsync(dto);

        if (!success)
            return BadRequest(new { message = "El correo ya está registrado" });

        return Ok(new { confirmation_code = true });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var success = await _authService.LoginAsync(dto);

        if (!success)
            return Unauthorized(new { confirmation_code = false });

        return Ok(new { confirmation_code = true });
    }
}
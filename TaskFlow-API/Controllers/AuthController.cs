using Microsoft.AspNetCore.Mvc;
using TaskFlow_API.DTOs;
using TaskFlow_API.Services;

namespace TaskFlow_API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var fields = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(request.Name))
            fields["name"] = "is required";
        if (string.IsNullOrWhiteSpace(request.Email))
            fields["email"] = "is required";
        if (string.IsNullOrWhiteSpace(request.Password))
            fields["password"] = "is required";

        if (fields.Any())
        {
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = fields });
        }

        try
        {
            var response = await _authService.RegisterAsync(request);
            return Created(string.Empty, response);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = new Dictionary<string, string> { ["email"] = ex.Message } });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var fields = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(request.Email))
            fields["email"] = "is required";
        if (string.IsNullOrWhiteSpace(request.Password))
            fields["password"] = "is required";

        if (fields.Any())
        {
            return BadRequest(new ErrorResponse { Error = "validation failed", Fields = fields });
        }

        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (ApplicationException)
        {
            return Unauthorized(new ErrorResponse { Error = "invalid credentials" });
        }
    }
}
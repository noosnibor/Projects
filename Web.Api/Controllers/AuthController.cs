using Microsoft.AspNetCore.Mvc;
using Web.Api.Data;
using Web.Api.Domain;
using Web.Api.Security;
using Web.Api.Services;

namespace Web.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(UserRepository userRepository, RoleRepository roleRepository, TokenRepository tokenRepository, TokenService tokenService, EmailTokenService emailTokenService, EmailService emailService, IConfiguration configuration) : ControllerBase
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly RoleRepository _roleRepository = roleRepository;
    private readonly TokenRepository _tokenRepository = tokenRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly EmailTokenService _emailTokenService = emailTokenService;
    private readonly EmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Verify if the user exists
        var exists = await _userRepository.GetByEmail(request.Email);

        // Display message indicating that the user exists
        if (exists is not null) return Conflict("Email already registered");

        // Hash the user password
        var hash = PasswordHasher.HashPassword(request.Password);

        // Create user and get id
        await _userRepository.CreateUser(request.Email, hash);
        var userId = await _userRepository.GetByEmail(request.Email);

        // Default role
        var roleId = await _roleRepository.GetRoleIdByName("User");
        await _roleRepository.AssignRoles(userId.Id, roleId);

        // Email confirmation token
        var token = _emailTokenService.GenerateToken();
        var expires = DateTime.UtcNow.AddHours(24);
        await _tokenRepository.AddOneTimeToken(userId.Id, "email_confirm", token, expires);

        // Create email link and sending email
        var link = $"{_configuration["Email:BaseUrl"]}/api/auth/confirm-email?token={Uri.EscapeDataString(token)}";
        await _emailService.Send(request.Email, "Confirm your email", $"Click to confirm: <a href=\"{link}\">Confirm</a>");

        // Return success
        return Ok(new { Message = "Registered. Please confirm your email." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        // Consume Token
        var (oneTimeToken, userId) = await _tokenRepository.ConsumeOneTimeToken("email_confirm", token);

        // Validate parameters
        if (oneTimeToken is null || userId is null) return BadRequest("Invalid or expired token");

        // Update database that email was confirmed
        await _userRepository.ConfirmEmail(userId.Value);

        // Return success
        return Ok(new { Message = "Email Confirmed" });
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            return BadRequest("Authentication failed");
        }

        var claims = result.Principal?.Claims?.ToList();
        if (claims == null)
        {
            return BadRequest("No claims found");
        }

        // Générer un token JWT
        var token = GenerateJwtToken(claims);
        
        // Retourner le token (dans un vrai projet, vous pourriez rediriger vers votre frontend avec le token)
        return Ok(new { token = token });
    }

    [HttpPost("token")]
    public IActionResult GetToken([FromBody] LoginRequest request)
    {
        // Dans un vrai projet, vous valideriez les credentials ici
        // Pour cet exemple, nous créons un token basique
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Email),
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(JwtRegisteredClaimNames.Sub, request.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = GenerateJwtToken(claims);
        return Ok(new { token = token });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var user = HttpContext.User;
        return Ok(new
        {
            Name = user.FindFirst(ClaimTypes.Name)?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            Claims = user.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    private string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var secretKey = _configuration["Authentication:Jwt:SecretKey"];
        var issuer = _configuration["Authentication:Jwt:Issuer"];
        var audience = _configuration["Authentication:Jwt:Audience"];

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT configuration is incomplete");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
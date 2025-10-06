using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using EshopApi.Models;
using EshopApi.Services;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPasswordService _passwordService;

    public AuthController(IUserService userService, IPasswordService passwordService)
    {
        _userService = userService;
        _passwordService = passwordService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Validation des données
        if (string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest("Tous les champs sont requis");
        }

        // Validation de l'email
        if (!IsValidEmail(request.Email))
        {
            return BadRequest("Format d'email invalide");
        }

        // Validation du mot de passe
        if (request.Password.Length < 6)
        {
            return BadRequest("Le mot de passe doit contenir au moins 6 caractères");
        }

        try
        {
            // Vérifier si l'email existe déjà
            if (await _userService.EmailExistsAsync(request.Email))
            {
                return BadRequest("Un compte avec cet email existe déjà");
            }

            // Créer l'utilisateur
            var user = await _userService.CreateUserAsync(request);

            // Créer les claims pour l'authentification
            var claims = CreateUserClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            // Connecter l'utilisateur avec un cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Retourner la réponse sans token
            return Ok(new
            {
                Message = "Compte créé et connecté avec succès",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur lors de la création du compte");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validation des données
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email et mot de passe requis");
        }

        try
        {
            // Récupérer l'utilisateur
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("Email ou mot de passe incorrect");
            }

            // Vérifier le mot de passe
            if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return BadRequest("Email ou mot de passe incorrect");
            }

            // Créer les claims pour l'authentification
            var claims = CreateUserClaims(user);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            // Connecter l'utilisateur avec un cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Retourner la réponse sans token
            return Ok(new
            {
                Message = "Connexion réussie",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur lors de la connexion");
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userIdClaim = HttpContext.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur lors de la récupération du profil");
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Déconnexion réussie" });
        }
        catch (Exception)
        {
            return StatusCode(500, "Erreur lors de la déconnexion");
        }
    }

    private List<Claim> CreateUserClaims(User user)
    {
        return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("sub", user.Id.ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
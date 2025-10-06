using Dapper;
using EshopApi.Models;
using System.Data;

namespace EshopApi.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(RegisterRequest request);
    Task<bool> EmailExistsAsync(string email);
}

public class UserService : IUserService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IPasswordService _passwordService;

    public UserService(IDbConnectionFactory connectionFactory, IPasswordService passwordService)
    {
        _connectionFactory = connectionFactory;
        _passwordService = passwordService;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        const string query = @"
            SELECT id, email, first_name, last_name, password_hash, created_at, updated_at, is_active
            FROM users 
            WHERE email = @Email AND is_active = true";

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        const string query = @"
            SELECT id, email, first_name, last_name, password_hash, created_at, updated_at, is_active
            FROM users 
            WHERE id = @Id AND is_active = true";

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<User> CreateUserAsync(RegisterRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var hashedPassword = _passwordService.HashPassword(request.Password);
        var now = DateTime.UtcNow;

        const string query = @"
            INSERT INTO users (email, first_name, last_name, password_hash, created_at, updated_at, is_active)
            VALUES (@Email, @FirstName, @LastName, @PasswordHash, @CreatedAt, @UpdatedAt, @IsActive)
            RETURNING id, email, first_name, last_name, password_hash, created_at, updated_at, is_active";

        var user = await connection.QueryFirstAsync<User>(query, new
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = hashedPassword,
            CreatedAt = now,
            UpdatedAt = now,
            IsActive = true
        });

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        const string query = "SELECT COUNT(1) FROM users WHERE email = @Email";
        var count = await connection.QueryFirstAsync<int>(query, new { Email = email });
        
        return count > 0;
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationUsersAPI.Data;
using AuthenticationUsersAPI.DTOs;
using AuthenticationUsersAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationUsersAPI.Services;

public class AuthenticationUsersService : IAuthenticationUsersService 
{
    private readonly AccountDbContext _dbContext;
    private readonly IConfiguration _config;
    
    public AuthenticationUsersService(AccountDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }
    
    
    // [loginDto]
    // username: renniD                                       id | username | password_hash | role
    // password: qwerty125                                    1  | renniD   | diuasdp87ags87dgvaspu | user
    public async Task<ResponceLoginDto> LoginAsync(LoginDto loginDto)
    {
        var account = _dbContext.Accounts.FirstOrDefault(acc => acc.Username == loginDto.Username);
        if (account == null)
        {
            return null;
        }

        bool ok = BCrypt.Net.BCrypt.Verify(loginDto.Password, account.PasswordHash);

        if (ok == false) return null;
        
        // -----------------
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        var buildToken = new JwtSecurityTokenHandler().WriteToken(token);

        return ToDto(account, buildToken);
    }

    public async Task<ResponceAccountDto> RegisterAsync(CreateAccountDto createAccountDto, string role)
    {
        bool existLogin = await _dbContext.Accounts.AnyAsync(a => createAccountDto.Username == a.Username);

        if (existLogin) return null;
        
        var newAccount = new Account()
        {
            Username = createAccountDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createAccountDto.Password),
            Role = role ?? "user"
        };

        _dbContext.Accounts.Add(newAccount);
        await _dbContext.SaveChangesAsync();
        return ToDto(newAccount);

        return null;  // ToDto();
    }

    public string Search()
    {
        return "admin access"; 
    }

    private static ResponceAccountDto ToDto(Account user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Role = user.Role,
        ActiveStatus = user.ActiveStatus,
        UpdatedAt = user.UpdatedAt,
        CreatedAt = user.CreatedAt
    };

    private static ResponceLoginDto ToDto(Account user, string token) => new()
    {
        Id = user.Id, Username = user.Username, Role = user.Role, Token = token, ActiveStatus = user.ActiveStatus, UpdatedAt = user.UpdatedAt, CreatedAt = user.CreatedAt
    };
}
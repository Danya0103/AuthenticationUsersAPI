using AuthenticationUsersAPI.DTOs;
using AuthenticationUsersAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationUsersAPI.Controllers;


// roles: anonym; user; admin

[ApiController]
[Route("api/auth")]
public class AccountController : ControllerBase
{
    private readonly IAuthenticationUsersService _usersService;

    public AccountController(IAuthenticationUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] CreateAccountDto dto, string role)
    {
        var resultAccount = await _usersService.RegisterAsync(dto, role);

        if (resultAccount is null)
            return Conflict(new { message = $"Account create error" });
        
        return Ok(resultAccount);
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
    {
        var resultAccount = await _usersService.LoginAsync(dto);

        if (resultAccount is null)
            return Conflict(new { message = $"Account login error" });
        
        return Ok(resultAccount);
    }

    [HttpGet("search")]
    [Authorize(Roles = "admin")]
    public IActionResult Search()
    {
        var resultSearch =  _usersService.Search();

        return Ok(resultSearch);
    }
}
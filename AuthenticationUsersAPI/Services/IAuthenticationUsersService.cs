using AuthenticationUsersAPI.DTOs;

namespace AuthenticationUsersAPI.Services;

public interface IAuthenticationUsersService
{
    Task<ResponceAccountDto> RegisterAsync (CreateAccountDto createAccountDto, string role);
    Task<ResponceLoginDto> LoginAsync (LoginDto loginDto);

    string Search();
}
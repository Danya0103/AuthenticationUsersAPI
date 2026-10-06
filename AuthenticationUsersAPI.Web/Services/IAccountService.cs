using AuthenticationUsersAPI.Web.Models;

namespace AuthenticationUsersAPI.Web.Services;

public interface IAccountService
{
    Task<ApiResult> RegisterAsync(RegisterViewModel viewModel, string role);
    Task<ApiResult> LoginAsync(LoginViewModel viewModel);
}

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ApiResult> RegisterAsync(RegisterViewModel viewModel, string role)
    {
        // payload, response, content
        var payload = new
        {
            username = viewModel.Username,
            password = viewModel.Password,
            confirmPassword = viewModel.ConfirmPassword
        };

        var response = await _httpClient.PostAsJsonAsync($"api/auth/register/{role}", payload);

        var content = await response.Content.ReadAsStringAsync();

        return new ApiResult()
        {
            IsSuccessful = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            RawMessage = content
        };
    }
    
    public async Task<ApiResult> LoginAsync(LoginViewModel viewModel)
    {
        // payload, response, content
        var payload = new
        {
            username = viewModel.Username,
            password = viewModel.Password,
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", payload);

        var content = await response.Content.ReadAsStringAsync();

        return new ApiResult()
        {
            IsSuccessful = response.IsSuccessStatusCode,
            StatusCode = (int)response.StatusCode,
            RawMessage = content
        };
    }
}


public class ApiResult
{
    public bool IsSuccessful { get; set; }
    public int StatusCode { get; set; }
    public string? RawMessage { get; set; }
}
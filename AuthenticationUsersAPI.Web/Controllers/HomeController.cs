using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AuthenticationUsersAPI.Web.Models;
using AuthenticationUsersAPI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace AuthenticationUsersAPI.Web.Controllers;

public class HomeController : Controller
{
    private readonly IAccountService _accountService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HomeController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "user")]
    public IActionResult Page1()
    {
        return View();
    }
    
    // ----

    [HttpPost]
    public async Task<IActionResult> Registration(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        var result = await _accountService.RegisterAsync(registerViewModel, "user");

        if (result.IsSuccessful)
        {
            TempData["Message"] = "register ok";
            return View(nameof(Login));
        }
        
        ModelState.AddModelError(string.Empty, result.StatusCode is 409 or 500 ? "Server error" : "Something wrong");
        return View(registerViewModel);
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ErrorRegister()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);

        var result = await _accountService.LoginAsync(loginViewModel);

        if (!result.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, result.StatusCode is 409 or 500 ? "Server error" : "Something wrong");
            return View(loginViewModel);
        }
        
        // Json serialize: txt -> json
        //     de-       : json -> txt

        var loginResult = JsonSerializer.Deserialize<LoginResultViewModel>(result.RawMessage, JsonOptions);
        
        // claims, identity, principal

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, loginResult.Id.ToString()),
            new(ClaimTypes.Name, loginResult.Username),
            new(ClaimTypes.Role, loginResult.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties();
        
        authProperties.StoreTokens(new []
        {
            new AuthenticationToken{Name = "access_token", Value = loginResult.Token}
        });
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        
        TempData["Message"] = "login ok";
        return View(nameof(Index)); 
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ErrorLogin()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
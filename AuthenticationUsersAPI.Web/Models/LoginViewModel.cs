using System.ComponentModel.DataAnnotations;

namespace AuthenticationUsersAPI.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Your username does not meet the naming requirements")]
    [MinLength(2), MaxLength(32)]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Your password does not meet the naming requirements")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")] 
    public string Password { get; set; }
}
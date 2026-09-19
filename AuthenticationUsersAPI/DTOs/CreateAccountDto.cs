using System.ComponentModel.DataAnnotations;

namespace AuthenticationUsersAPI.DTOs;

public class CreateAccountDto
{
    [Required] [MinLength(2), MaxLength(32)]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")] 
    public string Password { get; set; }
    
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}
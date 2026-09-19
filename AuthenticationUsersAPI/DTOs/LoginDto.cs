using System.ComponentModel.DataAnnotations;

namespace AuthenticationUsersAPI.DTOs;

public class LoginDto
{
    [Required] [MinLength(2), MaxLength(32)]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")] 
    public string Password { get; set; }
}
namespace AuthenticationUsersAPI.DTOs;

public class ResponceLoginDto
{
    public int Id { get; set; }

    public string Username { get; set; }
    
    public string Role { get; set; }

    public bool? ActiveStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    
    public string Token { get; set; }
}
using Domain;

namespace Shared.Admin;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? CurrentLevel { get; set; }
}
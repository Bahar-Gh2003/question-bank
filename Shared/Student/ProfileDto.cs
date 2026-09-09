namespace Shared.Student;

public class ProfileDto
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string CurrentLevelTitle { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
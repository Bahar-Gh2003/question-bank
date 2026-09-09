namespace Domain;

public class User : BaseEntity
{
    public string FullName { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public UserRole Role { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; } // Stored as a Gregorian date
    public Guid? CurrentLevelId { get; set; }
    public Level? CurrentLevel { get; set; }
}

public enum Gender
{
    Male,
    Female
}
public enum UserRole
{
    Student,
    Admin
}
using Domain;

namespace Sahred.Admin;

public class AdminUpdateUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public int? BirthYear { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthDay { get; set; }
}
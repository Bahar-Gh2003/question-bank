using System.ComponentModel.DataAnnotations;
using Domain;

namespace Shared;

public class UserForRegistrationDto
{
    [MinLength(5, ErrorMessage = "نام کامل باید حداقل ۵ کاراکتر باشد.")]
    public string FullName { get; set; }
    
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    public string Username { get; set; }
    
    [EmailAddress(ErrorMessage = "لطفاً یک آدرس ایمیل معتبر وارد کنید.")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "جنسیت الزامی است")]
    public Gender? Gender { get; set; }
    
    [Required(ErrorMessage = "رمز عبور الزامی است")]
    public string Password { get; set; }
    
    public int? BirthYear { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthDay { get; set; }
}
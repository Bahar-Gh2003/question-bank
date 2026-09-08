using System.Globalization;
using Application.Contracts;
using Domain;
using MediatR;

namespace Application.Features.Authentication;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // 1. نوع بازگشتی از Task<Unit> به Task تغییر کرد
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _unitOfWork.UserRepository.FindFirstOrDefaultAsync(u => u.Username == request.Dto.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("این نام کاربری قبلاً استفاده شده است. لطفاً نام دیگری انتخاب کنید.");
        }

        // --- شروع کد جدید ---
        // ابتدا سطح ۱ را از دیتابیس پیدا می‌کنیم
        var levelOne = await _unitOfWork.LevelRepository.FindFirstOrDefaultAsync(l => l.LevelNumber == 1);
        if (levelOne == null)
        {
            // اگر سطح ۱ وجود نداشت، یک خطا می‌دهیم تا ادمین ابتدا آن را بسازد
            throw new Exception("سطح ۱ در دیتابیس یافت نشد. لطفاً ابتدا سطوح را ایجاد کنید.");
        }
        
        var dto = request.Dto;
        DateTime? dateOfBirth = null;
        if (dto.BirthYear.HasValue && dto.BirthMonth.HasValue && dto.BirthDay.HasValue)
        {
            try
            {
                var persianCalendar = new PersianCalendar();
                dateOfBirth = persianCalendar.ToDateTime(dto.BirthYear.Value, dto.BirthMonth.Value, dto.BirthDay.Value, 0, 0, 0, 0);
            }
            catch
            {
                throw new InvalidOperationException("تاریخ تولد وارد شده معتبر نیست.");
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Username = dto.Username,
            PhoneNumber = dto.PhoneNumber, // جایگزین ایمیل شد
            Gender = dto.Gender,           // فیلد جدید
            DateOfBirth = dateOfBirth,     // فیلد جدید
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Email = dto.Email,
            Role = UserRole.Student,
            CurrentLevelId = levelOne.Id
        };

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
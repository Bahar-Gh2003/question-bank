using Application.Contracts;
using MediatR;

namespace Application.Features.Authentication;


public interface ITokenGenerator
{
    string GenerateToken(Domain.User user);
}

public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthenticateUserQueryHandler(IUnitOfWork unitOfWork, ITokenGenerator tokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        // 1. پیدا کردن کاربر بر اساس نام کاربری
        // (نیاز به اضافه کردن متد GetByUsername به ریپازیتوری داریم)
// روش جدید و انعطاف‌پذیر برای پیدا کردن کاربر
        var user = await _unitOfWork.UserRepository.FindFirstOrDefaultAsync(u => u.Username == request.Dto.Username);
        if (user == null)
            throw new Exception("نام کاربری یا رمز عبور اشتباه است.");

        // 2. بررسی صحت رمز عبور
        if (!BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            throw new Exception("نام کاربری یا رمز عبور اشتباه است.");

        // 3. ساخت توکن در صورت موفقیت
        var token = _tokenGenerator.GenerateToken(user);

        return token;
    }
}
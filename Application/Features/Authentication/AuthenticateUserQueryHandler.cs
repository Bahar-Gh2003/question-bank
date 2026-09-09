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
        // 1. Find the user by username
        var user = await _unitOfWork.UserRepository.FindFirstOrDefaultAsync(u => u.Username == request.Dto.Username);
        if (user == null)
            throw new Exception("نام کاربری یا رمز عبور اشتباه است.");

        // 2. Verify the password
        if (!BCrypt.Net.BCrypt.Verify(request.Dto.Password, user.PasswordHash))
            throw new Exception("نام کاربری یا رمز عبور اشتباه است.");

        // 3. Issue a token on success
        var token = _tokenGenerator.GenerateToken(user);

        return token;
    }
}
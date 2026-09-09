using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Profile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, ProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyProfileQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId,
            include: u => u.Include(l => l.CurrentLevel));

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        return new ProfileDto
        {
            FullName = user.FullName,
            Username = user.Username,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            CurrentLevelTitle = user.CurrentLevel?.Title ?? "No Level",
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}
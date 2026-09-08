using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Admin;

namespace Application.Features.Users;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var students = await _unitOfWork.UserRepository.GetAllAsync(
            predicate: u => u.Role == UserRole.Student,
            include: u => u.Include(l => l.CurrentLevel)
        );

        return students.Select(s => new UserDto
        {
            Id = s.Id,
            FullName = s.FullName,
            Username = s.Username,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,
            CurrentLevel = s.CurrentLevel?.Title ?? "Not Assigned"
        }).ToList();
    }
}
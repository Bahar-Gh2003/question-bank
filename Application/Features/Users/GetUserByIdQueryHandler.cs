using System.Globalization;
using Application.Contracts;
using MediatR;
using Sahred.Admin;

namespace Application.Features.Users;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, AdminUpdateUserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AdminUpdateUserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        var dto = new AdminUpdateUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Gender,
        };
        if (user.DateOfBirth.HasValue)
        {
            var persianCalendar = new PersianCalendar();
            dto.BirthYear = persianCalendar.GetYear(user.DateOfBirth.Value);
            dto.BirthMonth = persianCalendar.GetMonth(user.DateOfBirth.Value);
            dto.BirthDay = persianCalendar.GetDayOfMonth(user.DateOfBirth.Value);
        }
        return dto;
    }
}
using System.Globalization;
using Application.Contracts;
using MediatR;

namespace Application.Features.Users;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateUserCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userToUpdate = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
        if (userToUpdate != null)
        {
            userToUpdate.FullName = request.FullName;
            userToUpdate.Username = request.Username;
            userToUpdate.PhoneNumber = request.PhoneNumber;
            userToUpdate.Gender = request.Gender;
            if (request.BirthYear.HasValue)
            {
                var p = new PersianCalendar();
                userToUpdate.DateOfBirth = p.ToDateTime(request.BirthYear.Value, request.BirthMonth.Value, request.BirthDay.Value, 0, 0, 0, 0);
            }
            _unitOfWork.UserRepository.Update(userToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
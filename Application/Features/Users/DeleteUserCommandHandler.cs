using Application.Contracts;
using MediatR;

namespace Application.Features.Users;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userToDelete = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (userToDelete != null)
        {
            // You might want to add logic here to delete related data first
            _unitOfWork.UserRepository.Delete(userToDelete);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
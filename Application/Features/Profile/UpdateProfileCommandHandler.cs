using Application.Contracts;
using MediatR;

namespace Application.Features.Profile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>

{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user != null)
        {
            user.FullName = request.FullName;
            user.Email = request.Email;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
using Application.Contracts;
using MediatR;

namespace Application.Features.Profile;

public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfilePictureCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (user != null)
        {
            user.ProfilePictureUrl = request.ImageUrl;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
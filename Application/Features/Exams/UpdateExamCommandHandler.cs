using Application.Contracts;
using MediatR;

namespace Application.Features.Exams;

public class UpdateExamCommandHandler : IRequestHandler<UpdateExamCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateExamCommand request, CancellationToken cancellationToken)
    {
        var examToUpdate = await _unitOfWork.ExamRepository.GetByIdAsync(request.Id);
        if (examToUpdate != null)
        {
            examToUpdate.Title = request.Title;
            examToUpdate.LevelId = request.LevelId;
            // examToUpdate.StartTime = request.StartTime.Value;
            examToUpdate.DurationInMinutes = request.DurationInMinutes;
            examToUpdate.PassingScore = request.PassingScore;
            
            _unitOfWork.ExamRepository.Update(examToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
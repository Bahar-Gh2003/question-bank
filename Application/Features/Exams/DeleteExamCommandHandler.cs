using Application.Contracts;
using MediatR;

namespace Application.Features.Exams;

public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteExamCommand request, CancellationToken cancellationToken)
    {
        var relatedAttempts = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.ExamId == request.ExamId
        );

        if (relatedAttempts.Any())
        {
            // 2. Refuse deletion if any attempt history exists
            throw new InvalidOperationException("این آزمون قابل حذف نیست زیرا دانشجویان در آن شرکت کرده‌اند.");
        }
        var examToDelete = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId);
        if (examToDelete != null)
        {
            _unitOfWork.ExamRepository.Delete(examToDelete);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
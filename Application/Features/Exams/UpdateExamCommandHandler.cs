using Application.Common;
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
        if (examToUpdate is null)
            throw new NotFoundException("آزمون یافت نشد.");

        if (request.QuestionCount < 1)
            throw new BusinessRuleException("تعداد سوالات آزمون باید حداقل ۱ باشد.");

        var availableQuestions = (await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == request.LevelId)).Count();

        if (availableQuestions < request.QuestionCount)
            throw new BusinessRuleException(
                $"این سطح تنها {availableQuestions} سوال دارد و نمی‌توان آزمونی با {request.QuestionCount} سوال ساخت.");

        examToUpdate.Title = request.Title;
        examToUpdate.LevelId = request.LevelId;
        examToUpdate.DurationInMinutes = request.DurationInMinutes;
        examToUpdate.PassingScore = request.PassingScore;
        examToUpdate.QuestionCount = request.QuestionCount;

        _unitOfWork.ExamRepository.Update(examToUpdate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
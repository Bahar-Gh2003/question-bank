using Application.Common;
using Application.Contracts;
using Domain;
using MediatR;

namespace Application.Features.Exams;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateExamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        if (!request.LevelId.HasValue)
            throw new BusinessRuleException("سطح باید مشخص شود.");

        if (request.QuestionCount < 1)
            throw new BusinessRuleException("تعداد سوالات آزمون باید حداقل ۱ باشد.");

        // Warn the admin now rather than letting students hit an empty exam later
        var availableQuestions = (await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == request.LevelId.Value)).Count();

        if (availableQuestions < request.QuestionCount)
            throw new BusinessRuleException(
                $"این سطح تنها {availableQuestions} سوال دارد و نمی‌توان آزمونی با {request.QuestionCount} سوال ساخت.");

        var exam = new Exam
        {
            Title = request.Title,
            LevelId = request.LevelId.Value,
            DurationInMinutes = request.DurationInMinutes,
            PassingScore = request.PassingScore,
            QuestionCount = request.QuestionCount
        };

        await _unitOfWork.ExamRepository.AddAsync(exam);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return exam.Id;
    }
}
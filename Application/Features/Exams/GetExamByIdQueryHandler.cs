using Application.Common;
using Application.Contracts;
using MediatR;
using Shared.Admin;

namespace Application.Features.Exams;

public class GetExamByIdQueryHandler : IRequestHandler<GetExamByIdQuery, UpdateExamDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetExamByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateExamDto> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId);
        if (exam is null)
            throw new NotFoundException("آزمون یافت نشد.");

        return new UpdateExamDto
        {
            Id = exam.Id,
            Title = exam.Title,
            LevelId = exam.LevelId,
            DurationInMinutes = exam.DurationInMinutes,
            PassingScore = exam.PassingScore,
            QuestionCount = exam.QuestionCount
        };
    }
}
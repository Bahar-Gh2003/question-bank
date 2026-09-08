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
        return new UpdateExamDto
        {
            Id = exam.Id,
            Title = exam.Title,
            LevelId = exam.LevelId,
            // StartTime = exam.StartTime,
            DurationInMinutes = exam.DurationInMinutes,
            PassingScore = exam.PassingScore
        };
    }
}
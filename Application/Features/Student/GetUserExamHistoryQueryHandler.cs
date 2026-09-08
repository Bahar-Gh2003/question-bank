using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Student;

namespace Application.Features.Student;

public class GetUserExamHistoryQueryHandler : IRequestHandler<GetUserExamHistoryQuery, List<ExamAttemptDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserExamHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ExamAttemptDto>> Handle(GetUserExamHistoryQuery request, CancellationToken cancellationToken)
    {
        var attempts = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.UserId == request.UserId,
            include: a => a.Include(e => e.Exam).ThenInclude(l => l.Level)
        );

        return attempts
            .GroupBy(a => a.ExamId)
            .SelectMany(group => group.OrderBy(a => a.AttemptedAt)
                .Select((a, index) => new ExamAttemptDto
                {
                    AttemptId = a.Id,
                    ExamTitle = a.Exam.Title,
                    LevelTitle = a.Exam.Level.Title,
                    AttemptedAt = a.AttemptedAt,
                    AttemptNumber = index + 1
                }))
            .OrderByDescending(a => a.AttemptedAt)
            .ToList();
    }
}
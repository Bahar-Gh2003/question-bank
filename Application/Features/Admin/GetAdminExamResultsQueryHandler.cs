using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred;
using Sahred.Admin;

namespace Application.Features.Admin;

public class GetAdminExamResultsQueryHandler : IRequestHandler<GetAdminExamResultsQuery, AdminExamResultsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAdminExamResultsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AdminExamResultsDto> Handle(GetAdminExamResultsQuery request, CancellationToken cancellationToken)
{
    var allAttemptsForExam = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
        predicate: a => a.ExamId == request.ExamId,
        include: a => a.Include(u => u.User)
    );

    var userAttempts = allAttemptsForExam
        .GroupBy(a => a.UserId)
        .Select(g => g.OrderBy(a => a.AttemptedAt).ToList())
        .ToList();

    var resultsDto = new AdminExamResultsDto();

    // --- منطق جدید رتبه‌بندی برای تلاش اول ---
    var attempt1List = userAttempts.Where(ua => ua.Count >= 1).Select(ua => ua[0]).OrderByDescending(a => a.Score).ToList();
    resultsDto.Attempt1Results = attempt1List.Select(a => new ExamResultRankDto
    {
        AttemptId = a.Id,
        Rank = attempt1List.FindIndex(item => item.Score == a.Score) + 1,
        FullName = a.User.FullName,
        Username = a.User.Username,
        Score = a.Score,
        IsPassed = a.IsPassed
    }).ToList();

    // --- منطق جدید رتبه‌بندی برای تلاش دوم ---
    var attempt2List = userAttempts.Where(ua => ua.Count >= 2).Select(ua => ua[1]).OrderByDescending(a => a.Score).ToList();
    resultsDto.Attempt2Results = attempt2List.Select(a => new ExamResultRankDto
    {
        AttemptId = a.Id,
        Rank = attempt2List.FindIndex(item => item.Score == a.Score) + 1,
        FullName = a.User.FullName,
        Username = a.User.Username,
        Score = a.Score,
        IsPassed = a.IsPassed
    }).ToList();
    
    // --- منطق جدید رتبه‌بندی برای تلاش سوم ---
    var attempt3List = userAttempts.Where(ua => ua.Count >= 3).Select(ua => ua[2]).OrderByDescending(a => a.Score).ToList();
    resultsDto.Attempt3Results = attempt3List.Select(a => new ExamResultRankDto
    {
        AttemptId = a.Id,
        Rank = attempt3List.FindIndex(item => item.Score == a.Score) + 1,
        FullName = a.User.FullName,
        Username = a.User.Username,
        Score = a.Score,
        IsPassed = a.IsPassed
    }).ToList();

    return resultsDto;
}

}
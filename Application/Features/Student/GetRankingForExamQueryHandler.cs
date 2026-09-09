using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Application.Features.Student;

public class GetRankingForExamQueryHandler : IRequestHandler<GetRankingForExamQuery, List<ExamResultRankDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRankingForExamQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ExamResultRankDto>> Handle(GetRankingForExamQuery request, CancellationToken cancellationToken)
    {
        var allAttemptsForExam = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.ExamId == request.ExamId,
            include: a => a.Include(u => u.User)
        );

        // Get only the latest attempt from each user
        var latestAttempts = allAttemptsForExam
            .GroupBy(a => a.UserId)
            .Select(g => g.OrderByDescending(a => a.AttemptedAt).First())
            .ToList();
        
        var rankedAttempts = latestAttempts.OrderByDescending(a => a.Score).ToList();
        var resultWithCorrectRank = new List<ExamResultRankDto>();
        // int rank = 0;
        // int lastScore = -1;
        
        for (int i = 0; i < rankedAttempts.Count; i++)
        {
            var attempt = rankedAttempts[i];
        
            // Each rank is the position of the first person who reached that score
            int rank = rankedAttempts.FindIndex(a => a.Score == attempt.Score) + 1;
        
            resultWithCorrectRank.Add(new ExamResultRankDto
            {
                AttemptId = attempt.Id,
                Rank = rank,
                FullName = attempt.User.FullName,
                Username = attempt.User.Username,
                Score = attempt.Score,
                IsPassed = attempt.IsPassed
            });
        }

        
        return resultWithCorrectRank;


        // Rank based on score
        // return latestAttempts
        //     .OrderByDescending(a => a.Score)
        //     .Select((a, index) => new ExamResultRankDto
        //     {
        //         AttemptId = a.Id,
        //         Rank = index + 1,
        //         FullName = a.User.FullName,
        //         Username = a.User.Username,
        //         Score = a.Score,
        //         IsPassed = a.IsPassed
        //     })
        //     .ToList();
    }
}
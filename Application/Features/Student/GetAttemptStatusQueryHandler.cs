using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Student;

public class GetAttemptStatusQueryHandler : IRequestHandler<GetAttemptStatusQuery, AttemptStatusDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAttemptStatusQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AttemptStatusDto> Handle(GetAttemptStatusQuery request, CancellationToken cancellationToken)
    {
        
        var studentAttempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(request.AttemptId,
            include: a => a.Include(e => e.Exam));

        if (studentAttempt == null || studentAttempt.UserId != request.UserId)
            throw new Exception("Attempt not found.");

        // 1. Get all attempts for this specific exam
        var allAttemptsForExam = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.ExamId == studentAttempt.ExamId
        );

        // 2. Group all attempts by user and assign an attempt number to each
        var attemptsWithNumbers = allAttemptsForExam
            .GroupBy(a => a.UserId)
            .SelectMany(g => g.OrderBy(a => a.AttemptedAt)
                              .Select((attempt, index) => new { Attempt = attempt, AttemptNumber = index + 1 }))
            .ToList();

        // 3. Find the attempt number for the specific attempt the student is viewing
        var studentAttemptInfo = attemptsWithNumbers.FirstOrDefault(a => a.Attempt.Id == request.AttemptId);
        if (studentAttemptInfo == null)
            throw new Exception("Could not determine attempt number.");
        
        var studentAttemptNumber = studentAttemptInfo.AttemptNumber;

        // 4. Filter the list to include only attempts with the same attempt number
        var relevantAttempts = attemptsWithNumbers
            .Where(a => a.AttemptNumber == studentAttemptNumber)
            .Select(a => a.Attempt)
            .ToList();

        // 5. Get all questions to calculate the total score
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => q.LevelId == studentAttempt.Exam.LevelId
        );
        var totalExamScore = questions.Sum(q => q.Score);

        // 6. Perform all calculations on the correctly filtered list
        var rankedAttempts = relevantAttempts.OrderByDescending(a => a.Score).ToList();
        var studentRank = rankedAttempts.FindIndex(a => a.UserId == studentAttempt.UserId) + 1;

        return new AttemptStatusDto
        {
            StudentScore = studentAttempt.Score,
            IsPassed = studentAttempt.IsPassed,
            TotalExamScore = totalExamScore,
            Rank = studentRank,
            TotalParticipants = rankedAttempts.Count,
            PassedCount = rankedAttempts.Count(a => a.IsPassed),
            FailedCount = rankedAttempts.Count - rankedAttempts.Count(a => a.IsPassed),
            PassingScore = studentAttempt.Exam.PassingScore,
            ScoreDistribution = rankedAttempts.GroupBy(a => a.Score).ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
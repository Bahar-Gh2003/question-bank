using Application.Common;
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
        var studentAttempt = await _unitOfWork.ExamAttemptRepository.GetByIdAsync(
            request.AttemptId,
            include: a => a.Include(e => e.Exam).Include(sa => sa.StudentAnswers));

        if (studentAttempt is null)
            throw new NotFoundException("سابقه آزمون یافت نشد.");

        if (studentAttempt.UserId != request.UserId)
            throw new ForbiddenException("شما به این سابقه آزمون دسترسی ندارید.");

        // Only finished attempts take part in ranking; an attempt still in progress
        // would otherwise show up with a score of zero.
        var allAttemptsForExam = (await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.ExamId == studentAttempt.ExamId && a.IsCompleted)).ToList();

        // Group all attempts by user and assign an attempt number to each
        var attemptsWithNumbers = allAttemptsForExam
            .GroupBy(a => a.UserId)
            .SelectMany(g => g.OrderBy(a => a.AttemptedAt)
                              .Select((attempt, index) => new { Attempt = attempt, AttemptNumber = index + 1 }))
            .ToList();

        var studentAttemptInfo = attemptsWithNumbers.FirstOrDefault(a => a.Attempt.Id == request.AttemptId);
        if (studentAttemptInfo is null)
            throw new NotFoundException("شماره تلاش این آزمون قابل تشخیص نیست.");

        // Compare like with like: only attempts of the same number are ranked together
        var relevantAttempts = attemptsWithNumbers
            .Where(a => a.AttemptNumber == studentAttemptInfo.AttemptNumber)
            .Select(a => a.Attempt)
            .ToList();

        // The maximum score comes from the questions THIS attempt contained.
        // Questions are drawn at random per attempt, so summing the whole level's
        // question pool would report a maximum the student was never asked for.
        var questionIds = studentAttempt.StudentAnswers.Select(sa => sa.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id));

        var totalExamScore = questions.Sum(q => q.Score);

        var rankedAttempts = relevantAttempts.OrderByDescending(a => a.Score).ToList();
        var studentRank = rankedAttempts.FindIndex(a => a.UserId == studentAttempt.UserId) + 1;

        return new AttemptStatusDto
        {
            StudentScore = studentAttempt.Score,
            IsPassed = studentAttempt.IsPassed,
            AwaitingGrading = !studentAttempt.IsGraded,
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
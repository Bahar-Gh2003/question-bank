using Application.Contracts;
using Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Student;

namespace Application.Features.Student;

public class GetAvailableExamsQueryHandler : IRequestHandler<GetAvailableExamsQuery, List<ExamListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAvailableExamsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<ExamListDto>> Handle(GetAvailableExamsQuery request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
        if (student?.CurrentLevelId is null) return new List<ExamListDto>();

        var currentLevel = await _unitOfWork.LevelRepository.GetByIdAsync(student.CurrentLevelId.Value);
        if (currentLevel is null) return new List<ExamListDto>();

        // Exams from the current level AND every level below it stay available,
        // so a student promoted early can still go back and take the ones they skipped.
        var exams = await _unitOfWork.ExamRepository.GetAllAsync(
            predicate: e => e.Level!.LevelNumber <= currentLevel.LevelNumber,
            include: e => e.Include(l => l.Level));

        var studentAttempts = await _unitOfWork.ExamAttemptRepository.GetAllAsync(
            predicate: a => a.UserId == request.UserId);

        // UTC everywhere - the same basis used for storage in the database
        var utcNow = DateTime.UtcNow;

        return exams.Select(exam =>
        {
            var attemptsForThisExam = studentAttempts.Where(a => a.ExamId == exam.Id).ToList();

            // Exactly the same calculation StartExamCommandHandler performs
            var eligibility = ExamEligibilityCalculator.Evaluate(exam, attemptsForThisExam, utcNow);

            return new ExamListDto
            {
                ExamId = exam.Id,
                Title = exam.Title,
                DurationInMinutes = exam.DurationInMinutes,
                QuestionCount = exam.QuestionCount,
                LevelTitle = exam.Level!.Title,
                LevelNumber = exam.Level.LevelNumber,
                AttemptsMade = attemptsForThisExam.Count(a => a.IsCompleted),
                Status = eligibility.Status,
                NextAttemptAvailableAt = eligibility.NextAvailableAtUtc
            };
        })
        .OrderBy(e => e.LevelNumber)
        .ThenBy(e => e.Title)
        .ToList();
    }
}
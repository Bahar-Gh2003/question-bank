using Application.Contracts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Student;

namespace Application.Features.Student;

public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, ExamResultDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitExamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ExamResultDto> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId, include: q => q.Include(e => e.Level));
        if (exam == null) throw new Exception("آزمون یافت نشد.");

        var attempt = new ExamAttempt
        {
            UserId = request.UserId,
            ExamId = request.ExamId,
            LevelId = exam.LevelId,
            AttemptedAt = DateTime.UtcNow
        };

        // ذخیره پاسخ‌ها
        foreach (var answer in request.MultipleChoiceAnswers)
        {
            attempt.StudentAnswers.Add(new StudentAnswer { QuestionId = answer.Key, SelectedOptionId = answer.Value });
        }
        foreach (var answer in request.ShortAnswerTexts)
        {
            attempt.StudentAnswers.Add(new StudentAnswer { QuestionId = answer.Key, ShortAnswerText = answer.Value });
        }
        
        await _unitOfWork.ExamAttemptRepository.AddAsync(attempt);

        // تصحیح آزمون
        int totalScore = 0;
        var questionIds = attempt.StudentAnswers.Select(a => a.QuestionId).ToList();
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync(
            predicate: q => questionIds.Contains(q.Id),
            include: q => q.Include(o => o.Options)
        );

        foreach (var studentAnswer in attempt.StudentAnswers)
        {
            var question = questions.First(q => q.Id == studentAnswer.QuestionId);
            if (question.Type == QuestionType.MultipleChoice)
            {
                var correctOptionId = question.Options.FirstOrDefault(o => o.IsCorrect)?.Id;
                studentAnswer.IsCorrect = studentAnswer.SelectedOptionId == correctOptionId;
                if (studentAnswer.IsCorrect == true)
                {
                    totalScore += question.Score;
                }
            }
        }
        
        attempt.Score = totalScore;
        attempt.IsPassed = totalScore >= exam.PassingScore;

        // منطق ارتقای سطح
        if (attempt.IsPassed)
        {
            var student = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            var nextLevel = await _unitOfWork.LevelRepository.FindFirstOrDefaultAsync(l => l.LevelNumber == exam.Level.LevelNumber + 1);
            if (student != null && nextLevel != null)
            {
                student.CurrentLevelId = nextLevel.Id;
                _unitOfWork.UserRepository.Update(student);
            }
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ExamResultDto { TotalScore = totalScore, IsPassed = attempt.IsPassed };
    }
}
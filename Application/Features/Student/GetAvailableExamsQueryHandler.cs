using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Student;

namespace Application.Features.Student;

public class GetAvailableExamsQueryHandler : IRequestHandler<GetAvailableExamsQuery, List<ExamListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAvailableExamsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

   public async Task<List<ExamListDto>> Handle(GetAvailableExamsQuery request, CancellationToken cancellationToken)
{
    var userId = request.UserId;
    var student = await _unitOfWork.UserRepository.GetByIdAsync(userId);
    if (student?.CurrentLevelId == null) return new List<ExamListDto>();

    var examsForLevel = await _unitOfWork.ExamRepository.GetAllAsync(
        predicate: e => e.LevelId == student.CurrentLevelId,
        include: e => e.Include(l => l.Level)
    );
    var studentAttempts = await _unitOfWork.ExamAttemptRepository.GetAllAsync(predicate: a => a.UserId == userId);
    var examList = new List<ExamListDto>();
    var now = DateTime.Now;

    foreach (var exam in examsForLevel)
    {
        var attemptsForThisExam = studentAttempts.Where(a => a.ExamId == exam.Id).ToList();
        var attemptCount = attemptsForThisExam.Count;
        var hasPassed = attemptsForThisExam.Any(a => a.IsPassed);
        string status = "نامشخص";
        DateTime? nextAvailable = null;

        if (hasPassed)
        {
            status = "قبول";
        }
        else
        {
            switch (attemptCount)
            {
                case 0: // تلاش اول
                    // var firstWindowEnd = exam.StartTime.AddMinutes(exam.DurationInMinutes);
                    // if (now >= exam.StartTime && now <= firstWindowEnd)
                    status = "آماده شروع";
                    // else if (now > firstWindowEnd)
                        // status = "فرصت از دست رفته";
                    // else
                    // {
                        // status = "قفل";
                        // nextAvailable = exam.StartTime;
                    // }
                    break;

                case 1: // تلاش دوم
                    // بلافاصله پس از تلاش اول فعال است
                    // var secondAttemptDeadline = exam.StartTime.AddMinutes(2 * exam.DurationInMinutes);
                    // var secondAttempt = attemptsForThisExam.Last().AttemptedAt.AddMinutes(exam.DurationInMinutes);
                    // if (now <= secondAttempt )
                    // {
                    var firstAttempt = attemptsForThisExam.First();
                    var secondAttemptDeadline = firstAttempt.AttemptedAt.AddHours(3).AddMinutes(exam.DurationInMinutes + 30);
                    if (now <= secondAttemptDeadline)
                    {
                        status = "آماده تلاش مجدد";
                    }
                    else
                    {
                        // اگر مهلت تمام شد، دوره انتظار ۴۸ ساعته برای تلاش سوم شروع می‌شود
                        status = "قفل";
                        // زمان فعال شدن تلاش سوم، ۴۸ ساعت پس از پایان مهلت تلاش دوم است
                        nextAvailable = secondAttemptDeadline.AddHours(48); 
                    }

                    if (now > nextAvailable)
                    {
                        status = "مردود";
                    }
                    // }
                    // else
                    // {
                        // status = "قفل";
                    // }
                    break;
                        
                case 2: // تلاش سوم
                    // nextAvailable = attemptsForThisExam.Last().AttemptedAt.AddHours(51).AddMinutes(30);
                    // if (now >= nextAvailable) status = "آماده تلاش مجدد (آخرین فرصت)";
                    // else status = "قفل";
                    // nextAvailable =exam.StartTime.AddMinutes(2 * exam.DurationInMinutes).AddHours(48) ;
                    // var thirdAttemptDeadline = exam.StartTime.AddMinutes(3 * exam.DurationInMinutes).AddHours(48);
                    // if (now >= nextAvailable && now <= thirdAttemptDeadline)
                        // status = "آماده تلاش مجدد (آخرین فرصت)";
                    // else if (now > thirdAttemptDeadline)
                        // status = "فرصت از دست رفته(تلاش سوم)";
                    // else
                    // {
                    //     status = "قفل";
                    // }
                    var secondAttemptTime = attemptsForThisExam.Last().AttemptedAt;
                    nextAvailable = secondAttemptTime.AddHours(51).AddMinutes(30);

                    if (now < nextAvailable)
                    {
                        status = "قفل";
                    }
                    else
                    {
                        var thirdAttemptDeadline = nextAvailable.Value.AddMinutes(exam.DurationInMinutes);
                        if (now <= thirdAttemptDeadline)
                        {
                            status = "آماده تلاش مجدد (آخرین فرصت)";
                        }
                        else
                        {
                            status = "مردود";
                        }
                    }
                    break;
                    
                default: // بیش از ۲ تلاش
                    status = "مردود";
                    break;
            }
        }

        // var examEndTime = exam.StartTime.AddMinutes(exam.DurationInMinutes);
        // if (attemptCount == 0 && now > examEndTime)
        // {
        //      status = "Missed";
        // }

        examList.Add(new ExamListDto
        {
            ExamId = exam.Id,
            Title = exam.Title,
            DurationInMinutes = exam.DurationInMinutes,
            LevelTitle = exam.Level.Title,
            // StartTime = exam.StartTime,
            AttemptsMade = attemptCount,
            Status = status,
            NextAttemptAvailableAt = nextAvailable
        });
    }

    return examList;
}
}
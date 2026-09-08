using Application.Contracts;
using Domain;
using MediatR;

namespace Application.Features.Exams;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateExamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        if (!request.LevelId.HasValue)
        {
            throw new Exception("سطح باید مشخص شود.");
        }

        // مشخص می‌کنیم که زمان ورودی ادمین، یک زمان محلی است
        // var localStartTime = DateTime.SpecifyKind(request.StartTime.Value, DateTimeKind.Local);
        // // آن زمان محلی را برای ذخیره‌سازی به استاندارد جهانی UTC تبدیل می‌کنیم
        // var utcStartTime = localStartTime.ToUniversalTime();

        var exam = new Exam
        {
            Title = request.Title,
            LevelId = request.LevelId.Value,
            // StartTime = request.StartTime.Value, 
            DurationInMinutes = request.DurationInMinutes,
            PassingScore = request.PassingScore
        };
        await _unitOfWork.ExamRepository.AddAsync(exam);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return exam.Id;
    }
        
}
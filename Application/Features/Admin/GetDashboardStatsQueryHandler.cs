using Application.Contracts;
using Domain;
using MediatR;
using Sahred.Admin;

namespace Application.Features.Admin;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(predicate: u => u.Role == UserRole.Student);
        var questions = await _unitOfWork.QuestionRepository.GetAllAsync();
        var exams = await _unitOfWork.ExamRepository.GetAllAsync();

        return new DashboardStatsDto
        {
            TotalUsers = users.Count,
            TotalQuestions = questions.Count,
            TotalExams = exams.Count
        };
        
    }

}
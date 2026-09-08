using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sahred.Admin;

namespace Application.Features.Exams;

public class GetAllExamsQueryHandler : IRequestHandler<GetAllExamsQuery, List<AdminExamListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllExamsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<AdminExamListDto>> Handle(GetAllExamsQuery request, CancellationToken cancellationToken)
    {
        var exams = await _unitOfWork.ExamRepository.GetAllAsync(include: e => e.Include(l => l.Level));
        return exams.Select(e => new AdminExamListDto
        {
            Id = e.Id,
            Title = e.Title,
            LevelTitle = e.Level.Title,
            // StartTime = e.StartTime,
            DurationInMinutes = e.DurationInMinutes
        }).OrderByDescending(e => e.LevelTitle).ToList();
    }
}
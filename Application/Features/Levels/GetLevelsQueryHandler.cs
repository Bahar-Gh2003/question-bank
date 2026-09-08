using Application.Contracts;
using MediatR;
using Shared.Admin;

namespace Application.Features.Levels;

public class GetLevelsQueryHandler : IRequestHandler<GetLevelsQuery, List<LevelDto>>

{
    private readonly IUnitOfWork _unitOfWork;

    public GetLevelsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<LevelDto>> Handle(GetLevelsQuery request, CancellationToken cancellationToken)
    {
        var levels = await _unitOfWork.LevelRepository.GetAllAsync();
        return levels.Select(l => new LevelDto
        {
            Id = l.Id,
            Title = l.Title
        }).OrderBy(l => l.Title).ToList();
    }
}
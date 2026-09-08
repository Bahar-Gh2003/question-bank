using MediatR;
using Shared.Admin;

namespace Application.Features.Levels;

public class GetLevelsQuery : IRequest<List<LevelDto>>
{
    
}
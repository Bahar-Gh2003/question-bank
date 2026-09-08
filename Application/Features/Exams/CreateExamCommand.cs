using MediatR;
using Shared.Admin;

namespace Application.Features.Exams;

public class CreateExamCommand : CreateExamDto, IRequest<Guid>
{
    
}
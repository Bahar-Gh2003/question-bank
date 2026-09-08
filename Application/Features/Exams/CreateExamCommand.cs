using MediatR;
using Sahred.Admin;

namespace Application.Features.Exams;

public class CreateExamCommand : CreateExamDto, IRequest<Guid>
{
    
}
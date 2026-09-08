using MediatR;
using Shared.Admin;

namespace Application.Features.Exams;

public class GetAllExamsQuery : IRequest<List<AdminExamListDto>>
{
    
}
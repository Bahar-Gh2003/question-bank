using MediatR;
using Sahred.Admin;

namespace Application.Features.Exams;

public class GetAllExamsQuery : IRequest<List<AdminExamListDto>>
{
    
}
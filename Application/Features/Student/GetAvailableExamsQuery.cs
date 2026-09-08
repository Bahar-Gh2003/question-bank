using MediatR;
using Sahred.Student;

namespace Application.Features.Student;
 
public class GetAvailableExamsQuery : IRequest<List<ExamListDto>>
{
    public Guid UserId { get; set; }
}
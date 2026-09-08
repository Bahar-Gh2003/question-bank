using MediatR;
using Shared.Admin;

namespace Application.Features.Questions;

public class GetAllQuestionsQuery :  IRequest<List<QuestionDto>>
{
    
}
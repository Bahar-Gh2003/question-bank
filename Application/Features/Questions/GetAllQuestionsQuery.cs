using MediatR;
using Sahred.Admin;

namespace Application.Features.Questions;

public class GetAllQuestionsQuery :  IRequest<List<QuestionDto>>
{
    
}
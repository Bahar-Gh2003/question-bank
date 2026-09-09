using Application.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Questions;

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var relatedAnswers = await _unitOfWork.StudentAnswerRepository.GetAllAsync(
            predicate: sa => sa.QuestionId == request.QuestionId
        );

        if (relatedAnswers.Any())
        {
            throw new System.InvalidOperationException("این سوال قابل حذف نیست زیرا توسط دانشجویان در آزمون پاسخ داده شده است.");
        }
        
        // 2. Load the question and its options
        var questionToDelete = await _unitOfWork.QuestionRepository.GetByIdAsync(request.QuestionId,
            include: q => q.Include(o => o.Options));
        
        if (questionToDelete != null)
        {
            // 3. Delete the options first
            var options = questionToDelete.Options.ToList();
            foreach (var option in options)
            {
                _unitOfWork.OptionRepository.Delete(option);
            }

            // 4. Then delete the question itself
            _unitOfWork.QuestionRepository.Delete(questionToDelete);
            
            // 5. Persist all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
    }

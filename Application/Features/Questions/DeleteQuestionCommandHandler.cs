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
        
        // ۲. سوال و گزینه‌های آن را پیدا می‌کنیم
        var questionToDelete = await _unitOfWork.QuestionRepository.GetByIdAsync(request.QuestionId,
            include: q => q.Include(o => o.Options));
        
        if (questionToDelete != null)
        {
            // ۳. ابتدا گزینه‌ها را حذف می‌کنیم
            var options = questionToDelete.Options.ToList();
            foreach (var option in options)
            {
                _unitOfWork.OptionRepository.Delete(option);
            }

            // ۴. سپس خود سوال را حذف می‌کنیم
            _unitOfWork.QuestionRepository.Delete(questionToDelete);
            
            // ۵. تمام تغییرات را ذخیره می‌کنیم
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
    }

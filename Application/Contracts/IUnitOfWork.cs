using Domain;

namespace Application.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Question> QuestionRepository { get; }
    IGenericRepository<Option> OptionRepository { get; }
    IGenericRepository<User> UserRepository { get; }
    IGenericRepository<Level> LevelRepository { get; }
    IGenericRepository<Exam> ExamRepository { get; }
    IGenericRepository<ExamAttempt> ExamAttemptRepository { get; }
    IGenericRepository<StudentAnswer> StudentAnswerRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
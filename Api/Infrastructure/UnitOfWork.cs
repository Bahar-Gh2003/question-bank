using Api.Data;
using Application.Contracts;
using Domain;

namespace Api.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IGenericRepository<Question>? _questionRepository;
    private IGenericRepository<User>? _userRepository;
    private IGenericRepository<Level>? _levelRepository;
    private IGenericRepository<Exam>? _examRepository;
    private IGenericRepository<ExamAttempt>? _examAttemptRepository;
    private IGenericRepository<Option>? _optionRepository;
    private IGenericRepository<StudentAnswer>? _studentAnswerRepository;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IGenericRepository<Question> QuestionRepository => _questionRepository ??= new GenericRepository<Question>(_context);
    public IGenericRepository<User> UserRepository => _userRepository ??= new GenericRepository<User>(_context);
    public IGenericRepository<Level> LevelRepository => _levelRepository ??= new GenericRepository<Level>(_context);
    public IGenericRepository<Exam> ExamRepository => _examRepository ??= new GenericRepository<Exam>(_context);
    public IGenericRepository<ExamAttempt> ExamAttemptRepository => _examAttemptRepository ??= new GenericRepository<ExamAttempt>(_context);
    public IGenericRepository<Option> OptionRepository => _optionRepository ??= new GenericRepository<Option>(_context);
    public IGenericRepository<StudentAnswer> StudentAnswerRepository => _studentAnswerRepository ??= new GenericRepository<StudentAnswer>(_context);

    
   

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    public void Dispose() => _context.Dispose();
}
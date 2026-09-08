using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Level> Levels { get; set; }
    DbSet<Question> Questions { get; set; }
    DbSet<Exam> Exams { get; set; }
    DbSet<ExamAttempt> ExamAttempts { get; set; }
    DbSet<Option> Options { get; set; }
    
    DbSet<StudentAnswer> StudentAnswers { get; set; }

    int SaveChanges(bool acceptAllChangesOnSuccess);
    int SaveChanges();
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
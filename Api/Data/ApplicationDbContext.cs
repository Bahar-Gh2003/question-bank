using Application.Contracts;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Level> Levels { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamAttempt> ExamAttempts { get; set; }
    public DbSet<StudentAnswer> StudentAnswers { get; set; }


    
}


    
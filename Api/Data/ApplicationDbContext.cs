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



    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     base.OnModelCreating(modelBuilder);
    //
    //     // این قانون به دیتابیس می‌گوید: اگر یک سوال حذف شد، تمام گزینه‌هایش را هم حذف کن
    //     modelBuilder.Entity<Option>()
    //         .HasOne(o => o.Question)
    //         .WithMany(q => q.Options)
    //         .HasForeignKey(o => o.QuestionId)
    //         .OnDelete(DeleteBehavior.Cascade);
    //
    //     // قانون ۲ (امن): اگر یک "تلاش در آزمون" حذف شد، پاسخ‌های دانشجو هم حذف شوند
    //     modelBuilder.Entity<StudentAnswer>()
    //         .HasOne(sa => sa.ExamAttempt)
    //         .WithMany(ea => ea.StudentAnswers)
    //         .HasForeignKey(sa => sa.ExamAttemptId)
    //         .OnDelete(DeleteBehavior.Cascade);
    //
    //     // قانون ۳ (مهم): برای جلوگیری از حلقه، بقیه روابط را محدود (Restrict) می‌کنیم
    //     // این یعنی شما نمی‌توانید یک سطح را حذف کنید، اگر هنوز آزمونی به آن متصل باشد
    //     modelBuilder.Entity<Exam>()
    //         .HasOne(e => e.Level)
    //         .WithMany()
    //         .HasForeignKey(e => e.LevelId)
    //         .OnDelete(DeleteBehavior.Restrict);
    //     
    //     modelBuilder.Entity<Question>()
    //         .HasOne(q => q.Level)
    //         .WithMany()
    //         .HasForeignKey(q => q.LevelId)
    //         .OnDelete(DeleteBehavior.Restrict);
    //     
    //     modelBuilder.Entity<ExamAttempt>()
    //         .HasOne(ea => ea.User)
    //         .WithMany()
    //         .HasForeignKey(ea => ea.UserId)
    //         .OnDelete(DeleteBehavior.Restrict);
    // }
}


    
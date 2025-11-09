using TaskOrganizer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Application.Interfaces;

namespace TaskOrganizer.Infrastructure.Context;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectUser> ProjectUsers => Set<ProjectUser>();
    public DbSet<TaskUser> TaskUsers => Set<TaskUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);

            entity.HasMany(p => p.Tasks)
                  .WithOne(t => t.Project!)
                  .HasForeignKey(t => t.ProjectId);
        });

        modelBuilder.Entity(TaskConfig());
        modelBuilder.Entity(TaskHistoryConfig());
        modelBuilder.Entity(TaskCommentConfig());
        modelBuilder.Entity(UserConfig());
        modelBuilder.Entity(ProjectUserConfig());
        modelBuilder.Entity(TaskUserConfig());
    }

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User>> UserConfig()
        => entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Position).IsRequired();
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProjectUser>> ProjectUserConfig()
        => entity =>
        {
            entity.ToTable("ProjectUsers");
            entity.HasKey(pu => new { pu.ProjectId, pu.UserId });

            entity.HasOne(pu => pu.User)
                  .WithMany(u => u.ProjectUsers)
                  .HasForeignKey(pu => pu.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskUser>> TaskUserConfig()
        => entity =>
        {
            entity.ToTable("TaskUsers");
            entity.HasKey(tu => new { tu.TaskId, tu.UserId });

            entity.HasOne(tu => tu.User)
                  .WithMany(u => u.TaskUsers)
                  .HasForeignKey(tu => tu.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskItem>> TaskConfig()
        => entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.Priority).IsRequired();            
            entity.HasMany(t => t.History)
                  .WithOne()
                  .HasForeignKey(h => h.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(t => t.Comments)
                  .WithOne()
                  .HasForeignKey(c => c.TaskItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskHistory>> TaskHistoryConfig()
        => entity =>
        {
            entity.ToTable("TaskHistories");
            entity.HasKey(h => h.Id);
        };

    private static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskComment>> TaskCommentConfig()
        => entity =>
        {
            entity.ToTable("TaskComments");
            entity.HasKey(c => c.Id);
        };
}

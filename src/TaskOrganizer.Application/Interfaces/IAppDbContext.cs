using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;

namespace TaskOrganizer.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<TaskHistory> TaskHistories { get; }
    DbSet<TaskComment> TaskComments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

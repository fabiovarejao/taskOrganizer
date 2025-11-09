using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Repositories;

public class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly AppDbContext _context;

    public TaskHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskHistory?> GetByIdAsync(Guid id)
    {
        return await _context.TaskHistories
            .FirstOrDefaultAsync(th => th.Id == id);
    }

    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.TaskHistories
            .Where(th => th.TaskItemId == taskId)
            .OrderByDescending(th => th.ChangedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.TaskHistories
            .Where(th => th.ChangedByUserId == userId)
            .OrderByDescending(th => th.ChangedAt)
            .ToListAsync();
    }

    public async Task AddAsync(TaskHistory taskHistory)
    {
        await _context.TaskHistories.AddAsync(taskHistory);
        await _context.SaveChangesAsync();
    }
}

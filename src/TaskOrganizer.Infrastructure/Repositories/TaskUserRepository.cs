using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Repositories;

public class TaskUserRepository : ITaskUserRepository
{
    private readonly AppDbContext _context;

    public TaskUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskUser?> GetByIdAsync(Guid taskId, Guid userId)
    {
        return await _context.TaskUsers
            .Include(tu => tu.User)
            .FirstOrDefaultAsync(tu => tu.TaskId == taskId && tu.UserId == userId);
    }

    public async Task<IEnumerable<TaskUser>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.TaskUsers
            .Include(tu => tu.User)
            .Where(tu => tu.TaskId == taskId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskUser>> GetByUserIdAsync(Guid userId)
    {
        return await _context.TaskUsers
            .Include(tu => tu.Task)
            .Where(tu => tu.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(TaskUser taskUser)
    {
        await _context.TaskUsers.AddAsync(taskUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskUser taskUser)
    {
        _context.TaskUsers.Remove(taskUser);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid taskId, Guid userId)
    {
        return await _context.TaskUsers
            .AnyAsync(tu => tu.TaskId == taskId && tu.UserId == userId);
    }
}

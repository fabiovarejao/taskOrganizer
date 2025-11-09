using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;
    public TaskRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(TaskItem task) => await _db.Tasks.AddAsync(task);

    // No explicit Update - EF tracks entity loaded via GetByIdAsync automatically
    public void Update(TaskItem task) { }

    public IQueryable<TaskItem> Query() => _db.Tasks
        .Include(t => t.History)
        .Include(t => t.Comments)
        .AsQueryable();

    public async Task<TaskItem?> GetByIdAsync(Guid id) => 
        await _db.Tasks
            .Include(t => t.History)
            .Include(t => t.Comments)
            .AsTracking() // Ensure tracking is enabled
            .FirstOrDefaultAsync(t => t.Id == id);

    public void Remove(TaskItem task) => _db.Tasks.Remove(task);

    public async Task SaveChangesAsync()
    {
        // Fix: Detect and correct incorrectly tracked History/Comment entities
        // When adding to a tracked parent collection, EF may incorrectly mark them as Modified
        foreach (var entry in _db.ChangeTracker.Entries<TaskHistory>().ToList())
        {
            // If marked as Modified but doesn't exist in DB (new entity), change to Added
            if (entry.State == EntityState.Modified)
            {
                var existsInDb = await _db.TaskHistories.AsNoTracking()
                    .AnyAsync(h => h.Id == entry.Entity.Id);
                if (!existsInDb)
                {
                    entry.State = EntityState.Added;
                }
            }
        }
        
        foreach (var entry in _db.ChangeTracker.Entries<TaskComment>().ToList())
        {
            // If marked as Modified but doesn't exist in DB (new entity), change to Added
            if (entry.State == EntityState.Modified)
            {
                var existsInDb = await _db.TaskComments.AsNoTracking()
                    .AnyAsync(c => c.Id == entry.Entity.Id);
                if (!existsInDb)
                {
                    entry.State = EntityState.Added;
                }
            }
        }
        
        await _db.SaveChangesAsync();
    }
}

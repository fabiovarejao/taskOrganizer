using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Interfaces;
using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Application.Services;

public class ReportsService : IReportsService
{
    private readonly IAppDbContext _db;

    public ReportsService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CompletedPerUserResult>> GetCompletedTasksPerUserAsync(int days = 30)
    {
        if (days <= 0) days = 30;
        var since = DateTime.UtcNow.AddDays(-days);
        var completed = TaskStatus.Completed.ToString();

        // Query without joining User table for now (UserId may be null in existing data)
        var q = await _db.TaskHistories
            .AsNoTracking()
            .Where(h => h.Field == "Status" && h.NewValue == completed && h.ChangedAt >= since)
            .GroupBy(h => h.ChangedByUserId)
            .Select(g => new CompletedPerUserResult(
                g.Key, 
                null, // UserName not available without proper User integration
                g.Count(), 
                Math.Round((double)g.Count() / days, 4)))
            .ToListAsync();

        return q;
    }
}

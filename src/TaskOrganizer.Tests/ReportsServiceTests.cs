using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Application.Services;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Tests;

public class ReportsServiceTests
{
    [Fact]
    public async Task GetCompletedTasksPerUserAsync_ReturnsCorrectAggregates()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "ReportsTestDb1")
            .Options;

        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();

        var now = DateTime.UtcNow;

        using (var ctx = new AppDbContext(options))
        {
            var project = new TaskOrganizer.Domain.Entities.Project { Name = "Projeto", UserId = Guid.NewGuid() };
            ctx.Projects.Add(project);
            var task1 = project.AddTask("T1", TaskPriority.Medium, userA);
            var task2 = project.AddTask("T2", TaskPriority.Low, userB);
            ctx.Tasks.AddRange(task1, task2);

            ctx.TaskHistories.Add(TaskHistory.Create(task1.Id, userA, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));
            ctx.TaskHistories.Add(TaskHistory.Create(task1.Id, userA, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));
            ctx.TaskHistories.Add(TaskHistory.Create(task2.Id, userB, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));

            await ctx.SaveChangesAsync();
        }

        using (var ctx = new AppDbContext(options))
        {
            var svc = new ReportsService(ctx);
            var results = await svc.GetCompletedTasksPerUserAsync(30);
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            var a = results.First(r => r.UserId == userA);
            var b = results.First(r => r.UserId == userB);
            Assert.Equal(2, a.CompletedCount);
            Assert.Equal(1, b.CompletedCount);
            Assert.Equal(Math.Round((double)2 / 30, 4), a.AveragePerDay);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Tests;

public class InfrastructureDbTests
{
    [Fact]
    public async Task AppDbContext_Can_Persist_Project_Task_History_And_Comment()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InfraTestDb1")
            .Options;

        using (var ctx = new AppDbContext(options))
        {
            var project = new Project { Name = "InfraProj", UserId = Guid.NewGuid() };
            ctx.Projects.Add(project);

            var task = project.AddTask("T1", TaskPriority.Medium, Guid.NewGuid());
            ctx.Tasks.Add(task);

            var history = TaskHistory.Create(task.Id, Guid.NewGuid(), "Status", "Pending", "InProgress");
            ctx.TaskHistories.Add(history);

            var comment = new TaskComment(task.Id, "ola", Guid.NewGuid());
            ctx.TaskComments.Add(comment);

            await ctx.SaveChangesAsync();
        }

        using (var ctx = new AppDbContext(options))
        {
            var projects = await ctx.Projects.Include(p => p.Tasks).ToListAsync();
            Assert.Single(projects);
            var p = projects[0];
            Assert.Equal("InfraProj", p.Name);
            Assert.Single(p.Tasks);

            var tasks = await ctx.Tasks.Include(t => t.History).Include(t => t.Comments).ToListAsync();
            Assert.Single(tasks);
            var t = tasks[0];
            
            var histories = await ctx.TaskHistories.Where(h => h.TaskItemId == t.Id).ToListAsync();
            Assert.Single(histories);
            var comments = await ctx.TaskComments.Where(c => c.TaskItemId == t.Id).ToListAsync();
            Assert.Single(comments);
        }
    }
}

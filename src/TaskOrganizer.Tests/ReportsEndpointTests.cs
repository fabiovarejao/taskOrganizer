using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Tests;

public class ReportsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ReportsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("environment", "Testing");
            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) || d.ServiceType == typeof(AppDbContext)).ToList();
                foreach (var d in descriptors) services.Remove(d);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("ReportsEndpointDb");
                });
            });
        });
    }

    [Fact]
    public async Task GetCompletedPerUser_ReturnsExpectedAggregates()
    {
        var client = _factory.CreateClient();

        Guid managerId = Guid.Empty;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var manager = new User("Usuario Gerente", Position.Manager);
            db.Users.Add(manager);
            await db.SaveChangesAsync();
            managerId = manager.Id;

            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();
            var project = new TaskOrganizer.Domain.Entities.Project { Name = "Projeto", UserId = Guid.NewGuid() };
            db.Projects.Add(project);
            var t1 = project.AddTask("T1", TaskPriority.Medium, userA);
            var t2 = project.AddTask("T2", TaskPriority.Medium, userB);
            db.Tasks.AddRange(t1, t2);

            db.TaskHistories.Add(TaskHistory.Create(t1.Id, userA, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));
            db.TaskHistories.Add(TaskHistory.Create(t1.Id, userA, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));
            db.TaskHistories.Add(TaskHistory.Create(t2.Id, userB, "Status", "Pending", TaskOrganizer.Domain.Enums.TaskStatus.Completed.ToString()));

            await db.SaveChangesAsync();
        }

        var resp = await client.GetAsync($"/reports/completed-per-user?userId={managerId}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var list = await resp.Content.ReadFromJsonAsync<List<CompletedPerUserDto>>();
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        var a = list.First(x => x.CompletedCount == 2);
        var b = list.First(x => x.CompletedCount == 1);
        Assert.True(a.AveragePerDay > 0);
    }
}

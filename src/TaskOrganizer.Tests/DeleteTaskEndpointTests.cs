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

public class DeleteTaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeleteTaskEndpointTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("TestDb_DeleteTasks");
                });
            });
        });
    }

    [Fact]
    public async Task DeleteTask_ExistingTask_ReturnsNoContent()
    {
        
        var client = _factory.CreateClient();
        
        Guid taskId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userId = Guid.NewGuid();
            var project = new Project { Name = "Projeto Teste para Exclusao", UserId = userId };
            var task = project.AddTask("Tarefa para Exclusao", TaskPriority.Medium, userId);
            taskId = task.Id;
            db.Projects.Add(project);
            await db.SaveChangesAsync();
        }

        
        var response = await client.DeleteAsync($"/tasks/{taskId}");

        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deletedTask = await db.Tasks.FindAsync(taskId);
            Assert.Null(deletedTask);
        }
    }

    [Fact]
    public async Task DeleteTask_NonExistingTask_ReturnsNotFound()
    {
        
        var client = _factory.CreateClient();
        var nonExistingTaskId = Guid.NewGuid();

        
        var response = await client.DeleteAsync($"/tasks/{nonExistingTaskId}");

        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_TaskWithComments_DeletesSuccessfully()
    {
        
        var client = _factory.CreateClient();
        
        Guid taskId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userId = Guid.NewGuid();
            var project = new Project { Name = "Projeto Teste com Comentarios", UserId = userId };
            var task = project.AddTask("Tarefa com Comentarios para Exclusao", TaskPriority.High, userId);
            task.AddComment("Primeiro comentario", userId);
            task.AddComment("Segundo comentario", userId);
            taskId = task.Id;
            db.Projects.Add(project);
            await db.SaveChangesAsync();
        }

        
        var response = await client.DeleteAsync($"/tasks/{taskId}");

        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var deletedTask = await db.Tasks.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == taskId);
            Assert.Null(deletedTask);
        }
    }
}

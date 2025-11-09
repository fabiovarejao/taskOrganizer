using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Tests;

public class TaskUpdateEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public TaskUpdateEndpointTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("TaskUpdateTestsDb");
                });
            });
        });
    }

    [Fact]
    public async Task UpdateTask_Title_Description_DueDate_Status_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("ProjAtualizacao", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var project = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(project);

    var createTask = new CreateTaskDto("T-Atualizar", TaskPriority.Medium, userId, null, null);
        var taskResp = await client.PostAsJsonAsync($"/projects/{project!.Id}/tasks", createTask);
        Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
    var task = await taskResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(task);

    var newTitle = "T-Atualizar-NOVO";
    var newDesc = "descricao atualizada";
        var newDue = DateTime.UtcNow.Date.AddDays(7);
    var newStatus = TaskStatus.Completed;
        var updateDto = new UpdateTaskDto(newTitle, newDesc, newDue, null, newStatus);

        var putResp = await client.PutAsJsonAsync($"/tasks/{task!.Id}?userId={userId}", updateDto);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

    var updated = await putResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal(newTitle, updated!.Title);
        Assert.Equal(newDesc, updated.Description);
        Assert.Equal(newDue, updated.DueDate);
        Assert.Equal(newStatus, updated.Status);
        
        var getResp = await client.GetAsync($"/tasks/{updated.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
    var fetched = await getResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(fetched);
        Assert.Equal(newTitle, fetched!.Title);
        Assert.Equal(newStatus, fetched.Status);
    }

    [Fact]
    public async Task UpdateTask_AttemptChangePriority_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("ProjAtualizacao2", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var project = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(project);

    var createTask = new CreateTaskDto("T-Imutavel", TaskPriority.Medium, userId, null, null);
        var taskResp = await client.PostAsJsonAsync($"/projects/{project!.Id}/tasks", createTask);
        Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
    var task = await taskResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(task);
        
        var updateDto = new UpdateTaskDto(task!.Title, task.Description, task.DueDate, TaskPriority.High, null);
        var putResp = await client.PutAsJsonAsync($"/tasks/{task.Id}?userId={userId}", updateDto);
        Assert.Equal(HttpStatusCode.BadRequest, putResp.StatusCode);
        var body = await putResp.Content.ReadAsStringAsync();
    Assert.Contains("prioridade", body, StringComparison.OrdinalIgnoreCase);
    }
}

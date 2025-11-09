using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Domain.Enums;
using Xunit;

namespace TaskOrganizer.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public IntegrationTests(WebApplicationFactory<Program> factory)
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
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
    }

    [Fact]
    public async Task CreateProject_InvalidPayload_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var invalid = new CreateProjectDto("", Guid.Empty, "desc");
        var resp = await client.PostAsJsonAsync("/projects", invalid);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_NonExistentTask_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var randomTaskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var putResp = await client.PutAsync($"/tasks/{randomTaskId}/status?newStatus=2&userId={userId}", null);
        Assert.Equal(HttpStatusCode.NotFound, putResp.StatusCode);
    }

    [Fact]
    public async Task AddComment_NonExistentTask_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var randomTaskId = Guid.NewGuid();
        var commentDto = new CreateCommentDto("hi", Guid.NewGuid());
        var resp = await client.PostAsJsonAsync($"/tasks/{randomTaskId}/comments", commentDto);
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task CreateTask_NonExistentProject_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var projectId = Guid.NewGuid();
    var taskDto = new CreateTaskDto("T-NE", TaskPriority.Low, Guid.NewGuid(), null, null);
        var resp = await client.PostAsJsonAsync($"/projects/{projectId}/tasks", taskDto);
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_Succeeds_When_NoPendingTasks()
    {
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("ProjetoVazio", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(createdProject);

        var delResp = await client.DeleteAsync($"/projects/{createdProject.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);
    }

    [Fact]
    public async Task GetProjectTasks_Empty_ReturnsEmptyList()
    {
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("ProjetoSemTarefas", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(createdProject);

        var tasksResp = await client.GetAsync($"/projects?userId={userId}");
        Assert.Equal(HttpStatusCode.OK, tasksResp.StatusCode);
    var projects = await tasksResp.Content.ReadFromJsonAsync<List<ProjectDto>>(JsonOptions);
        Assert.NotNull(projects);
        Assert.Contains(projects!, p => p.Id == createdProject.Id);

        var getTasks = await client.GetAsync($"/projects/{createdProject.Id}/tasks");
        Assert.Equal(HttpStatusCode.OK, getTasks.StatusCode);
        var tasks = await getTasks.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task CreateProject_ThenCreateTask_ReturnsCreated()
    {
        var client = _factory.CreateClient();

    var projectDto = new CreateProjectDto("Projeto de Integracao", Guid.NewGuid(), "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        if (projectResp.StatusCode != HttpStatusCode.Created)
        {
            var body = await projectResp.Content.ReadAsStringAsync();
            throw new Xunit.Sdk.XunitException($"Falha ao criar projeto. Status: {projectResp.StatusCode}. Corpo: {body}");
        }
    var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(createdProject);

    var taskDto = new CreateTaskDto("T1", TaskPriority.Medium, Guid.NewGuid(), null, null);
        var taskResp = await client.PostAsJsonAsync($"/projects/{createdProject!.Id}/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
    }

    [Fact]
    public async Task AddTwentyTasks_ThenTwentyFirst_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

    var projectDto = new CreateProjectDto("Projeto Limite", Guid.NewGuid(), "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(createdProject);

        
        for (int i = 0; i < 20; i++)
        {
            var taskDto = new CreateTaskDto($"T{i}", TaskPriority.Medium, Guid.NewGuid(), null, null);
            var taskResp = await client.PostAsJsonAsync($"/projects/{createdProject!.Id}/tasks", taskDto);
            Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
        }

        
    var overflowDto = new CreateTaskDto("Excesso", TaskPriority.Low, Guid.NewGuid(), null, null);
        var overflowResp = await client.PostAsJsonAsync($"/projects/{createdProject!.Id}/tasks", overflowDto);
        Assert.Equal(HttpStatusCode.BadRequest, overflowResp.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_Blocked_When_PendingTasks_Then_Succeeds_After_Completing()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("Projeto para Exclusao", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
        var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(createdProject);

    var taskDto = new CreateTaskDto("Task1", TaskPriority.Medium, userId, null, null);
        var taskResp = await client.PostAsJsonAsync($"/projects/{createdProject!.Id}/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
    var createdTask = await taskResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(createdTask);

        
        var delResp = await client.DeleteAsync($"/projects/{createdProject.Id}");
        Assert.Equal(HttpStatusCode.BadRequest, delResp.StatusCode);


        
        var putResp = await client.PutAsync($"/tasks/{createdTask!.Id}/status?newStatus=2&userId={userId}", null);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        
        var delResp2 = await client.DeleteAsync($"/projects/{createdProject.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp2.StatusCode);
    }

    [Fact]
    public async Task TaskHistory_Includes_Status_And_Comment_With_ChangedByUserId()
    {
        var client = _factory.CreateClient();

        var userId = Guid.NewGuid();
    var projectDto = new CreateProjectDto("Historico do Projeto", userId, "desc");
        var projectResp = await client.PostAsJsonAsync("/projects", projectDto);
        Assert.Equal(HttpStatusCode.Created, projectResp.StatusCode);
    var createdProject = await projectResp.Content.ReadFromJsonAsync<ProjectDto>(JsonOptions);
        Assert.NotNull(createdProject);

    var taskDto = new CreateTaskDto("TaskHistoryTest", TaskPriority.Medium, userId, null, null);
        var taskResp = await client.PostAsJsonAsync($"/projects/{createdProject!.Id}/tasks", taskDto);
        Assert.Equal(HttpStatusCode.Created, taskResp.StatusCode);
    var createdTask = await taskResp.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(createdTask);

        
        var putResp = await client.PutAsync($"/tasks/{createdTask!.Id}/status?newStatus=2&userId={userId}", null);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        
        var commentDto = new CreateCommentDto("Este é um comentário", userId);
        var commentResp = await client.PostAsJsonAsync($"/tasks/{createdTask.Id}/comments", commentDto);
        Assert.Equal(HttpStatusCode.Created, commentResp.StatusCode);

        
        var historyResp = await client.GetAsync($"/tasks/{createdTask.Id}/history");
        Assert.Equal(HttpStatusCode.OK, historyResp.StatusCode);
        var content = await historyResp.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(content);
        var root = doc.RootElement;
        Assert.True(root.ValueKind == System.Text.Json.JsonValueKind.Array);

        bool foundStatus = false;
        bool foundComment = false;
        foreach (var item in root.EnumerateArray())
        {
            var field = item.GetProperty("field").GetString();
            var changedBy = item.GetProperty("changedByUserId").GetString();
            if (field == "Status" && changedBy == userId.ToString()) foundStatus = true;
            if (field == "Comment" && changedBy == userId.ToString()) foundComment = true;
        }

    Assert.True(foundStatus, "Registro de historico de Status com ChangedByUserId correto nao encontrado");
    Assert.True(foundComment, "Registro de historico de Comentario com ChangedByUserId correto nao encontrado");
    }
}

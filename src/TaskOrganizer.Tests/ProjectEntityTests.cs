using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using Xunit;

namespace TaskOrganizer.Tests;

public class ProjectEntityTests
{
    [Fact]
    public void Project_TaskLimit_ShouldDefaultTo20()
    {
    var project = new Project { Name = "Projeto Teste", UserId = Guid.NewGuid() };

        Assert.Equal(20, project.TaskLimit);
    }

    [Fact]
    public void Project_UpdateTaskLimit_ShouldUpdateCorrectly()
    {
    var project = new Project { Name = "Projeto Teste", UserId = Guid.NewGuid() };

        project.UpdateTaskLimit(50);

        Assert.Equal(50, project.TaskLimit);
    }

    [Fact]
    public void Project_UpdateTaskLimit_ShouldThrowException_WhenLessThan1()
    {
    var project = new Project { Name = "Projeto Teste", UserId = Guid.NewGuid() };

        Assert.Throws<ArgumentException>(() => project.UpdateTaskLimit(0));
        Assert.Throws<ArgumentException>(() => project.UpdateTaskLimit(-1));
    }

    [Fact]
    public void TaskItem_SetResponsibleUser_ShouldSetCorrectly()
    {
    var project = new Project { Name = "P", UserId = Guid.NewGuid() };
    var task = project.AddTask("Tarefa Teste", TaskPriority.High, Guid.NewGuid());
        var userId = Guid.NewGuid();

        task.SetResponsibleUser(userId);

        Assert.Equal(userId, task.ResponsibleUserId);
    }

    [Fact]
    public void TaskItem_SetResponsibleUser_ShouldAllowNull()
    {
    var project = new Project { Name = "P", UserId = Guid.NewGuid() };
    var task = project.AddTask("Tarefa Teste", TaskPriority.High, Guid.NewGuid());

    task.SetResponsibleUser(null);

        Assert.Null(task.ResponsibleUserId);
    }
}

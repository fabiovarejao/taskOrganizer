using TaskOrganizer.Domain.Entities;
using Xunit;

namespace TaskOrganizer.Tests;

public class ProjectUserTests
{
    [Fact]
    public void ProjectUser_Constructor_ShouldCreateProjectUser()
    {
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var projectUser = new ProjectUser(projectId, userId);

        Assert.Equal(projectId, projectUser.ProjectId);
        Assert.Equal(userId, projectUser.UserId);
    }

    [Fact]
    public void TaskUser_Constructor_ShouldCreateTaskUser()
    {
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var taskUser = new TaskUser(taskId, userId);

        Assert.Equal(taskId, taskUser.TaskId);
        Assert.Equal(userId, taskUser.UserId);
    }
}

using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Tests;

public class TaskItemTests
{
    [Fact]
    public void Priority_Is_Immutable_After_Creation()
    {
    var project = new Project { Name = "P", UserId = Guid.NewGuid() };
        var userId = Guid.NewGuid();

        var task = project.AddTask("T1", TaskPriority.High, userId);

    task.UpdateStatus(TaskStatus.InProgress, userId);
    task.UpdateDescription("nova descricao", userId);
    task.AddComment("comentário", userId);

        Assert.Equal(TaskPriority.High, task.Priority);
    }

    [Fact]
    public void History_Is_Generated_On_Status_Update_And_Comment()
    {
    var project = new Project { Name = "P", UserId = Guid.NewGuid() };
        var userId = Guid.NewGuid();

        var task = project.AddTask("T1", TaskPriority.Medium, userId);

    Assert.Empty(task.History);

    var before = DateTime.UtcNow;
        task.UpdateStatus(TaskStatus.InProgress, userId);
        var afterStatus = task.History.LastOrDefault();
        Assert.NotNull(afterStatus);
        Assert.Equal("Status", afterStatus.Field);
        Assert.Equal(TaskStatus.Pending.ToString(), afterStatus.OldValue);
        Assert.Equal(TaskStatus.InProgress.ToString(), afterStatus.NewValue);
        Assert.Equal(userId, afterStatus.ChangedByUserId);
        Assert.True(afterStatus.ChangedAt >= before && afterStatus.ChangedAt <= DateTime.UtcNow.AddSeconds(5));

    var commentBefore = DateTime.UtcNow;
    task.AddComment("ola", userId);
        var commentEntry = task.History.LastOrDefault();
        Assert.NotNull(commentEntry);
        Assert.Equal("Comment", commentEntry.Field);
        Assert.Null(commentEntry.OldValue);
    Assert.Equal("ola", commentEntry.NewValue);
        Assert.Equal(userId, commentEntry.ChangedByUserId);
        Assert.True(commentEntry.ChangedAt >= commentBefore && commentEntry.ChangedAt <= DateTime.UtcNow.AddSeconds(5));
    }
}

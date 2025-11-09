namespace TaskOrganizer.Domain.Entities;

public class TaskUser
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }

    public virtual TaskItem Task { get; set; } = null!;
    
    public virtual User User { get; set; } = null!;
    
    private TaskUser() { }

    public TaskUser(Guid taskId, Guid userId)
    {
        TaskId = taskId;
        UserId = userId;
    }
}

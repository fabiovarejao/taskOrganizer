namespace TaskOrganizer.Domain.Entities;

public class TaskHistory
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TaskItemId { get; private set; }
    public string Field { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string? Message { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid ChangedByUserId { get; private set; }

    private TaskHistory() { }

    private TaskHistory(Guid taskItemId, Guid changedByUserId, string field, string? oldValue, string? newValue)
    {
        TaskItemId = taskItemId;
        ChangedByUserId = changedByUserId;
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedAt = DateTime.UtcNow;
    }
    
    public static TaskHistory Create(Guid taskItemId, Guid changedByUserId, string field, string? oldValue, string? newValue)
        => new(taskItemId, changedByUserId, field, oldValue, newValue);
}


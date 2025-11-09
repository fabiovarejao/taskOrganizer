namespace TaskOrganizer.Domain.Entities;

public class TaskComment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TaskItemId { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }

    private TaskComment() { }

    public TaskComment(Guid taskItemId, string message, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new Exceptions.DomainException("Comentário não pode estar vazio.");
        
        TaskItemId = taskItemId;
        Message = message;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}

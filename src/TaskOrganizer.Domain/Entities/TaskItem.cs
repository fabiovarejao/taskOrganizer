using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProjectId { get; private set; }
    public Project? Project { get; private set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; private set; }
    
    public Guid? ResponsibleUserId { get; set; }

    public List<TaskHistory> History { get; private set; } = new List<TaskHistory>();
    public List<TaskComment> Comments { get; private set; } = new List<TaskComment>();
    
    public virtual ICollection<TaskUser> TaskUsers { get; private set; } = new List<TaskUser>();

    private TaskItem() { }

    public TaskItem(Guid projectId, TaskPriority priority )
    {
        ProjectId = projectId;        
        Priority = priority;        
        Status = TaskStatus.Pending;
    }

    public void UpdateStatus(TaskStatus newStatus, Guid userId)
    {
        var old = Status;
        Status = newStatus;
        History.Add(TaskHistory.Create(Id, userId, "Status", old.ToString(), newStatus.ToString()));
    }

    public void UpdateTitle(string title, Guid userId)
    {
        var old = Title;
        Title = title;
        History.Add(TaskHistory.Create(Id, userId, "Title", old, title));
    }

    public void UpdateDescription(string? description, Guid userId)
    {
        var old = Description;
        Description = description;
        History.Add(TaskHistory.Create(Id, userId, "Description", old, description));
    }

    public void UpdateDueDate(DateTime? dueDate, Guid userId)
    {
        var old = DueDate;
        DueDate = dueDate;
        History.Add(TaskHistory.Create(Id, userId, "DueDate", old?.ToString("yyyy-MM-dd"), dueDate?.ToString("yyyy-MM-dd")));
    }

    public void AddComment(string message, Guid userId)
    {
        var comment = new TaskComment(Id, message, userId);
        Comments.Add(comment);
        History.Add(TaskHistory.Create(Id, userId, "Comment", null, message));
    }

    public void SetResponsibleUser(Guid? userId)
    {
        ResponsibleUserId = userId;
    }
}

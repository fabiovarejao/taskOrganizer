using TaskOrganizer.Domain.Enums;
using TaskStatus = TaskOrganizer.Domain.Enums.TaskStatus;

namespace TaskOrganizer.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Guid UserId { get; set; }
    
    public int TaskLimit { get; private set; } = 20;

    public List<TaskItem> Tasks { get; private set; } = new List<TaskItem>();
    
    public virtual ICollection<ProjectUser> ProjectUsers { get; private set; } = new List<ProjectUser>();
    
    public Project()
    {
        
    }

    public TaskItem AddTask(string title, TaskPriority priority, Guid responsibleUserId, string? description = null, DateTime? dueDate = null)
    {
        if (Tasks.Count >= TaskLimit)
            throw new Exceptions.DomainException($"O projeto atingiu o número máximo de tarefas ({TaskLimit}).");

        var task = new TaskItem(Id, priority) { Title = title, Description = description, DueDate = dueDate };        
        task.SetResponsibleUser(responsibleUserId);
        Tasks.Add(task);
        return task;
    }

    public void UpdateTaskLimit(int newLimit)
    {
        if (newLimit < 1)
            throw new ArgumentException("Limite de tarefas deve ser pelo menos 1", nameof(newLimit));
        
        TaskLimit = newLimit;
    }
    
    public bool HasPendingTasks()
    {
        return Tasks.Any(t => t.Status == TaskStatus.Pending);
    }
}

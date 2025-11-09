namespace TaskOrganizer.Domain.Entities;

public class ProjectUser
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public virtual Project Project { get; set; } = null!;
    
    public virtual User User { get; set; } = null!;
    
    private ProjectUser() { }

    public ProjectUser(Guid projectId, Guid userId)
    {
        ProjectId = projectId;
        UserId = userId;
    }
}

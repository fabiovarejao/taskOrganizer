using TaskOrganizer.Domain.Enums;

namespace TaskOrganizer.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserName { get; private set; } = string.Empty;
    public Position Position { get; private set; }

    public virtual ICollection<ProjectUser> ProjectUsers { get; private set; } = new List<ProjectUser>();
    
    public virtual ICollection<TaskUser> TaskUsers { get; private set; } = new List<TaskUser>();

    // EF Constructor
    private User() { }

    public User(string userName, Position position)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Nome de usuário não pode estar vazio", nameof(userName));
        
        UserName = userName;
        Position = position;
    }

    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Nome de usuário não pode estar vazio", nameof(userName));
        
        UserName = userName;
    }

    public void UpdatePosition(Position position)
    {
        Position = position;
    }
}

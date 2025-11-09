using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Domain.Exceptions;

namespace TaskOrganizer.Tests;

public class ProjectTests
{
    [Fact]
    public void AddTask_WhenExceedsLimit_ThrowsDomainException()
    {
    var project = new Project() {Name = "Meu Projeto", UserId = Guid.NewGuid() };       

        for (int i = 0; i < 20; i++)
        {
            project.AddTask($"Tarefa {i}", TaskPriority.Medium, Guid.NewGuid());
        }

    var ex = Assert.Throws<DomainException>(() => project.AddTask("Tarefa Excedente", TaskPriority.Low, Guid.NewGuid()));
        Assert.Contains("número máximo de tarefas", ex.Message);
    }
}

using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Seed;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        if (await db.Projects.AnyAsync()) return;

        var demoUser = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var project = new Project()
        {
            Name = "Projeto de Demonstração",
            UserId = demoUser,
            Description = "Projeto de demonstração carregado"
        };
        var task = project.AddTask("Tarefa de Boas-Vindas", TaskPriority.Medium, demoUser, "Esta é uma tarefa carregada", DateTime.UtcNow.AddDays(7));
        await db.Projects.AddAsync(project);
        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();
    }
}

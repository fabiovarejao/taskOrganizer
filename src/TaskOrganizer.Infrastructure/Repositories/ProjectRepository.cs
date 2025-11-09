using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;
    public ProjectRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Project project) => await _db.Projects.AddAsync(project);

    public IQueryable<Project> Query() => _db.Projects.AsQueryable();

    public async Task<Project?> GetByIdAsync(Guid id) => await _db.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);

    public void Remove(Project project) => _db.Projects.Remove(project);

    public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
}

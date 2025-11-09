using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Interfaces.Repositories;
using TaskOrganizer.Infrastructure.Context;

namespace TaskOrganizer.Infrastructure.Repositories;

public class ProjectUserRepository : IProjectUserRepository
{
    private readonly AppDbContext _context;

    public ProjectUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectUser?> GetByIdAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectUsers
            .Include(pu => pu.User)
            .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);
    }

    public async Task<IEnumerable<ProjectUser>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectUsers
            .Include(pu => pu.User)
            .Where(pu => pu.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectUser>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ProjectUsers
            .Include(pu => pu.Project)
            .Where(pu => pu.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(ProjectUser projectUser)
    {
        await _context.ProjectUsers.AddAsync(projectUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProjectUser projectUser)
    {
        _context.ProjectUsers.Remove(projectUser);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectUsers
            .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);
    }
}

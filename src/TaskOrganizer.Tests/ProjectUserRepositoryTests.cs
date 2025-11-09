using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Infrastructure.Repositories;
using Xunit;

namespace TaskOrganizer.Tests;

public class ProjectUserRepositoryTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProjectUser()
    {
        using var context = GetInMemoryContext();
        var repo = new ProjectUserRepository(context);
        var projectId = Guid.NewGuid();

        var user = new User("test.user", Position.Manager);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var projectUser = new ProjectUser(projectId, user.Id);

        await repo.AddAsync(projectUser);

        var saved = await context.ProjectUsers.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal(projectId, saved.ProjectId);
        Assert.Equal(user.Id, saved.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProjectUser()
    {
        using var context = GetInMemoryContext();
        var repo = new ProjectUserRepository(context);
        var projectId = Guid.NewGuid();
        
        var user = new User("test.user", Position.Manager);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var projectUser = new ProjectUser(projectId, user.Id);
        await repo.AddAsync(projectUser);

        var retrieved = await repo.GetByIdAsync(projectId, user.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(projectId, retrieved.ProjectId);
        Assert.Equal(user.Id, retrieved.UserId);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueWhenAssignmentExists()
    {
        using var context = GetInMemoryContext();
        var repo = new ProjectUserRepository(context);
        var projectId = Guid.NewGuid();
        
        var user = new User("test.user", Position.Manager);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var projectUser = new ProjectUser(projectId, user.Id);
        await repo.AddAsync(projectUser);

        var exists = await repo.ExistsAsync(projectId, user.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalseWhenAssignmentDoesNotExist()
    {
        using var context = GetInMemoryContext();
        var repo = new ProjectUserRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProjectUser()
    {
        using var context = GetInMemoryContext();
        var repo = new ProjectUserRepository(context);
        var projectId = Guid.NewGuid();
        
        var user = new User("test.user", Position.Manager);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        var projectUser = new ProjectUser(projectId, user.Id);
        await repo.AddAsync(projectUser);

        await repo.DeleteAsync(projectUser);

        var deleted = await repo.GetByIdAsync(projectId, user.Id);
        Assert.Null(deleted);
    }
}

using Microsoft.EntityFrameworkCore;
using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using TaskOrganizer.Infrastructure.Context;
using TaskOrganizer.Infrastructure.Repositories;
using Xunit;

namespace TaskOrganizer.Tests;

public class UserRepositoryTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("test.user", Position.Manager);

        await repo.AddAsync(user);

        var saved = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal("test.user", saved.UserName);
        Assert.Equal(Position.Manager, saved.Position);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("test.user", Position.Analyst);
        await repo.AddAsync(user);

        var retrieved = await repo.GetByIdAsync(user.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(user.Id, retrieved.Id);
        Assert.Equal("test.user", retrieved.UserName);
    }

    [Fact]
    public async Task GetByUserNameAsync_ShouldReturnUser()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("joao.silva", Position.Specialist);
        await repo.AddAsync(user);

        var retrieved = await repo.GetByUserNameAsync("joao.silva");

        Assert.NotNull(retrieved);
        Assert.Equal("joao.silva", retrieved.UserName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("nome.velho", Position.Manager);
        await repo.AddAsync(user);

        user.UpdateUserName("nome.novo");
        user.UpdatePosition(Position.Analyst);
        await repo.UpdateAsync(user);

        var updated = await repo.GetByIdAsync(user.Id);
        Assert.NotNull(updated);
        Assert.Equal("nome.novo", updated.UserName);
        Assert.Equal(Position.Analyst, updated.Position);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("para.deletar", Position.Manager);
        await repo.AddAsync(user);

        await repo.DeleteAsync(user);

        var deleted = await repo.GetByIdAsync(user.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueWhenUserExists()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User("existe", Position.Manager);
        await repo.AddAsync(user);

        var exists = await repo.ExistsAsync(user.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalseWhenUserDoesNotExist()
    {
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid());

        Assert.False(exists);
    }
}

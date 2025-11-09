using TaskOrganizer.Domain.Entities;
using TaskOrganizer.Domain.Enums;
using Xunit;

namespace TaskOrganizer.Tests;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldCreateUser()
    {
    var user = new User("joao.silva", Position.Manager);

    Assert.Equal("joao.silva", user.UserName);
        Assert.Equal(Position.Manager, user.Position);
        Assert.NotNull(user.ProjectUsers);
        Assert.NotNull(user.TaskUsers);
        Assert.Empty(user.ProjectUsers);
        Assert.Empty(user.TaskUsers);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_Constructor_ShouldThrowException_WhenUserNameIsInvalid(string? userName)
    {
        Assert.Throws<ArgumentException>(() => new User(userName!, Position.Analyst));
    }

    [Fact]
    public void User_UpdateUserName_ShouldUpdateCorrectly()
    {
    var user = new User("joao.silva", Position.Manager);

        user.UpdateUserName("maria.silva");

        Assert.Equal("maria.silva", user.UserName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_UpdateUserName_ShouldThrowException_WhenUserNameIsInvalid(string? userName)
    {
    var user = new User("joao.silva", Position.Manager);

        Assert.Throws<ArgumentException>(() => user.UpdateUserName(userName!));
    }

    [Theory]
    [InlineData(Position.Manager)]
    [InlineData(Position.Analyst)]
    [InlineData(Position.Specialist)]
    public void User_UpdatePosition_ShouldUpdateCorrectly(Position newPosition)
    {
    var user = new User("joao.silva", Position.Manager);

        user.UpdatePosition(newPosition);

        Assert.Equal(newPosition, user.Position);
    }

    [Fact]
    public void Position_Enum_ShouldHaveCorrectValues()
    {
        Assert.Equal(0, (int)Position.Manager);
        Assert.Equal(1, (int)Position.Analyst);
        Assert.Equal(2, (int)Position.Specialist);
    }
}

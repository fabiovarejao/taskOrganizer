using FluentValidation;
using TaskOrganizer.Application.Dtos;
using TaskOrganizer.Application.Validators;
using TaskOrganizer.Domain.Enums;
using Xunit;

namespace TaskOrganizer.Tests;

public class ValidatorsTests
{
    [Fact]
    public void CreateProjectDtoValidator_Validates_Correctly()
    {
        var validator = new CreateProjectDtoValidator();
    var dto = new CreateProjectDto("Projeto", Guid.NewGuid(), "desc");
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);

        var invalid = new CreateProjectDto("", Guid.Empty, new string('a', 2000));
        var invRes = validator.Validate(invalid);
        Assert.False(invRes.IsValid);
        Assert.Contains(invRes.Errors, e => e.PropertyName == "Name");
        Assert.Contains(invRes.Errors, e => e.PropertyName == "UserId");
        Assert.Contains(invRes.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void CreateTaskDtoValidator_Validates_Correctly()
    {
        var validator = new CreateTaskDtoValidator();
    var dto = new CreateTaskDto("Tarefa", TaskPriority.Medium, Guid.NewGuid(), null, null);
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);

        var invalid = new CreateTaskDto("", TaskPriority.Medium, Guid.Empty, null, null);
        var invRes = validator.Validate(invalid);
        Assert.False(invRes.IsValid);
        Assert.Contains(invRes.Errors, e => e.PropertyName == "Title");
        Assert.Contains(invRes.Errors, e => e.PropertyName == "ResponsibleUserId");
    }

    [Fact]
    public void CreateCommentDtoValidator_Validates_Correctly()
    {
        var validator = new CreateCommentDtoValidator();
    var dto = new CreateCommentDto("Ola", Guid.NewGuid());
        var result = validator.Validate(dto);
        Assert.True(result.IsValid);

        var invalid = new CreateCommentDto("", Guid.Empty);
        var invRes = validator.Validate(invalid);
        Assert.False(invRes.IsValid);
        Assert.Contains(invRes.Errors, e => e.PropertyName == "Message");
        Assert.Contains(invRes.Errors, e => e.PropertyName == "UserId");
    }
}

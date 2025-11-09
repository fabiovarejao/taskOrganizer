using FluentValidation;
using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ResponsibleUserId).NotEmpty();
        RuleFor(x => x.Priority).IsInEnum();
    }
}

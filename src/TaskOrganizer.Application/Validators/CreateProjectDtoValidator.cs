using FluentValidation;
using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Validators;

public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}

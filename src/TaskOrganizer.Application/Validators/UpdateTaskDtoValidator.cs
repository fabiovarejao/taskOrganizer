using FluentValidation;
using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Validators;

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título da tarefa é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

using FluentValidation;
using TaskOrganizer.Application.Dtos;

namespace TaskOrganizer.Application.Validators;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.UserId).NotEmpty();
    }
}

using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class CreatePositionDtoValidator : AbstractValidator<CreatePositionDto>
{
    public CreatePositionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Position title is required")
            .MaximumLength(255).WithMessage("Position title must not exceed 255 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}


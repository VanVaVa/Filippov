using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class CreateSkillDtoValidator : AbstractValidator<CreateSkillDto>
{
    public CreateSkillDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MaximumLength(255).WithMessage("Skill name must not exceed 255 characters");
    }
}


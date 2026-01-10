using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class UpdateSkillDtoValidator : AbstractValidator<UpdateSkillDto>
{
    public UpdateSkillDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Skill name is required")
            .MaximumLength(255).WithMessage("Skill name must not exceed 255 characters");
    }
}


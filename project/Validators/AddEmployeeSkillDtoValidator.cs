using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class AddEmployeeSkillDtoValidator : AbstractValidator<AddEmployeeSkillDto>
{
    public AddEmployeeSkillDtoValidator()
    {
        RuleFor(x => x.SkillId)
            .GreaterThan(0).WithMessage("Skill ID must be greater than 0");
    }
}


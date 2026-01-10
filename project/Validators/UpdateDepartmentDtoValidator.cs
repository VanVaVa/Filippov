using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required")
            .MaximumLength(255).WithMessage("Department name must not exceed 255 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Department code is required")
            .MaximumLength(50).WithMessage("Department code must not exceed 50 characters");
    }
}


using FluentValidation;
using HRPlatform.DTO.Requests;

namespace HRPlatform.Validators;

public class UpdateVacancyDtoValidator : AbstractValidator<UpdateVacancyDto>
{
    public UpdateVacancyDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0");

        RuleFor(x => x.PositionId)
            .GreaterThan(0).WithMessage("Position ID must be greater than 0");

        RuleFor(x => x.SalaryRange)
            .MaximumLength(100).WithMessage("Salary range must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.SalaryRange));
    }
}


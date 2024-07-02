using FluentValidation;
using TeamManagement.Application.RequestsResponse.Member;

namespace TeamManagement.Application.Validation.Member;

public class AddMemberValidator : AbstractValidator<AddMemberRequest>
{
    public AddMemberValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage(ValidationMessages.NameNotNull)
            .NotEmpty().WithMessage(ValidationMessages.NameNotEmpty);

        RuleFor(x => x.SalaryPerYear)
            .GreaterThan(0).WithMessage(ValidationMessages.SalaryNotZero);

        RuleFor(x => x.CountryName)
            .NotNull().WithMessage(ValidationMessages.CountryNotNull)
            .NotEmpty().WithMessage(ValidationMessages.CountryNotEmpty);
        
    }
}
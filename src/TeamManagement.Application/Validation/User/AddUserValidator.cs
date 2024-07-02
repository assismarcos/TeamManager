using FluentValidation;
using TeamManagement.Application.RequestsResponse.User;
using TeamManagement.Application.Validation.Member;

namespace TeamManagement.Application.Validation.User;

public class AddUserValidator : AbstractValidator<AddUserRequest>
{
    public AddUserValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull().WithMessage("User name cannot be null")
            .NotEmpty().WithMessage("USer name cannot be empty");
    }
}
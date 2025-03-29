using FastEndpoints;
using FluentValidation;

namespace Agency.Service.Authentication;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x=>x.UserId)
            .NotEmpty()
            .WithMessage("Please specify a user ID");
        RuleFor(x=>x.Password)
            .NotEmpty()
            .WithMessage("Please specify a password");
    }
}
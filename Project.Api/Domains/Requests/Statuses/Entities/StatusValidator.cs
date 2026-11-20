namespace Project.Api;

public class StatusValidator : AbstractValidator<Status>
{
    public StatusValidator()
    {
        RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must be less than 100 characters.");

    }
}
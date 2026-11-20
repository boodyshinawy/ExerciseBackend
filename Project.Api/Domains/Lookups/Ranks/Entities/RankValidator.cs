namespace Project.Api;

public class RankValidator : AbstractValidator<Rank>
{
    public RankValidator()
    {
        RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must be less than 100 characters.");

    }
}
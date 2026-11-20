namespace Project.Api;

public class PageValidator : AbstractValidator<Page>
{
    public PageValidator()
    {
        RuleFor(x => x.Label)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must be less than 100 characters.");

        RuleFor(x => x.To)
            .MaximumLength(100).WithMessage("Route must be less than 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.To));

        RuleFor(x => x.Icon)
            .MaximumLength(50).WithMessage("Icon name must be less than 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Icon));

        RuleFor(x => x.SortNumber)
            .GreaterThanOrEqualTo(0).WithMessage("Sort number must be greater than or equal to 0.");
    }
}
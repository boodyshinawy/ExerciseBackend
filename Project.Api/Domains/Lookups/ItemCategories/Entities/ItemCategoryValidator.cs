namespace Project.Api;

public class ItemCategoryValidator : AbstractValidator<ItemCategory>
{
    public ItemCategoryValidator()
    {
        RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must be less than 100 characters.");

    }
}
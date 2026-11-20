namespace Project.Api;

public class SaleEntityValidator : AbstractValidator<SaleEntity>
{
    public SaleEntityValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم جهة البيع مطلوب")
            .MaximumLength(200);
    }
}

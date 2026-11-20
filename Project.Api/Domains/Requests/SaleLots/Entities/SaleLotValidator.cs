namespace Project.Api;

public class SaleLotValidator : AbstractValidator<SaleLot>
{
    public SaleLotValidator()
    {
        RuleFor(x => x.SaleEntityId).GreaterThan(0);
    }
}

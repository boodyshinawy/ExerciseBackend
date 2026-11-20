namespace Project.Api;

public class SeizureRequestValidator : AbstractValidator<SeizureRequest>
{
    public SeizureRequestValidator()
    {
        RuleFor(x => x.MahdrNumber)
            .NotEmpty().WithMessage("رقم المحضر مطلوب");

        RuleFor(x => x.CaseNumber)
            .NotEmpty().WithMessage("رقم القضية مطلوب");

        RuleFor(x => x.RequestDate)
            .NotEmpty().WithMessage("تاريخ الطلب مطلوب");
    }
}
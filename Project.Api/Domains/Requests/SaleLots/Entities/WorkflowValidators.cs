namespace Project.Api;

public class CreateSaleLotValidator : AbstractValidator<CreateSaleLotDto>
{
    public CreateSaleLotValidator()
    {
        RuleFor(x => x.SaleEntityId).GreaterThan(0).WithMessage("جهة البيع مطلوبة");
        RuleFor(x => x.Items).NotEmpty().WithMessage("يجب اختيار مضبوطات للوط");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.SeizureRequestDetailId).GreaterThan(0);
            item.RuleFor(i => i.AllocatedQuantity).GreaterThan(0).WithMessage("الكمية المخصصة يجب أن تكون أكبر من صفر");
        });
    }
}

public class ReturnSeizureValidator : AbstractValidator<ReturnSeizureDto>
{
    public ReturnSeizureValidator()
    {
        RuleFor(x => x.SeizureRequestId).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().WithMessage("سبب الإرجاع إلزامي");
    }
}

public class BulkApproveValidator : AbstractValidator<BulkApproveDto>
{
    public BulkApproveValidator()
    {
        RuleFor(x => x.SeizureRequestIds).NotEmpty().WithMessage("يجب اختيار ضبطية واحدة على الأقل");
        RuleFor(x => x.AmanaAcceptNumber).NotEmpty().WithMessage("رقم تصديق الأمانة مطلوب");
        RuleFor(x => x.AmanaAcceptDate).NotNull().WithMessage("تاريخ تصديق الأمانة مطلوب");
        RuleFor(x => x.PermanentCommitteeNumber).NotEmpty().WithMessage("رقم اللجنة الدائمة مطلوب");
        RuleFor(x => x.PermanentCommitteeDate).NotNull().WithMessage("تاريخ انعقاد اللجنة الدائمة مطلوب");
    }
}

public class LotSettlementValidator : AbstractValidator<LotSettlementDto>
{
    public LotSettlementValidator()
    {
        RuleFor(x => x.TotalAmountCollected).GreaterThan(0).WithMessage("إجمالي المبلغ المحصل مطلوب");
        RuleFor(x => x.Receipt).NotNull().WithMessage("إيصال الإيداع البنكي إلزامي");
    }
}

public class LotFinanceRejectValidator : AbstractValidator<LotFinanceDecisionDto>
{
    public LotFinanceRejectValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("سبب الرفض إلزامي");
    }
}

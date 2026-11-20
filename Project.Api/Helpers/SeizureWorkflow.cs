namespace Project.Api;

public static class SeizureWorkflow
{
    public const string EntitySeizureDetail = "SeizureRequestDetail";
    public const string EntitySaleLot = "SaleLot";
    public const string EntitySeizureRequest = "SeizureRequest";

    public static bool IsImmutable(int statusId) =>
        statusId >= (int)SeizureStatus.SalesEntity;

    public static bool CanAllocate(int statusId) =>
        statusId is (int)SeizureStatus.SalesEntity
            or (int)SeizureStatus.PartiallyAllocated;

    public static bool CanAssignToServicesAgency(int statusId) =>
        statusId is (int)SeizureStatus.SalesEntity
            or (int)SeizureStatus.ServicesAgency
            or (int)SeizureStatus.PartiallyAllocated
            or (int)SeizureStatus.FullyAllocated;

    public static string SeizureStatusName(int statusId) => statusId switch
    {
        (int)SeizureStatus.Registration => "قيد التسجيل",
        (int)SeizureStatus.SubCommittee => "اللجنة الفرعية",
        (int)SeizureStatus.PermanentCommittee => "اللجنة الدائمة",
        (int)SeizureStatus.SalesEntity => "جهة البيع",
        (int)SeizureStatus.ServicesAgency => "جهاز الخدمات العامة",
        (int)SeizureStatus.PartiallyAllocated => "مخصص جزئياً",
        (int)SeizureStatus.FullyAllocated => "مخصص بالكامل",
        (int)SeizureStatus.FinanceAuthority => "هيئة شئون مالية",
        (int)SeizureStatus.Completed => "مكتمل",
        _ => "غير معروف"
    };

    public static string LotStatusName(int statusId) => statusId switch
    {
        (int)SaleLotStatus.DispatchedToSaleEntity => "لدى جهة البيع",
        (int)SaleLotStatus.SubmittedToFinance => "بانتظار الاعتماد المالي",
        (int)SaleLotStatus.Settled => "تم التسوية",
        (int)SaleLotStatus.Rejected => "مرفوض",
        _ => "غير معروف"
    };

    public static StatusHistory History(
        string entityType,
        int entityId,
        int fromStatusId,
        int toStatusId,
        string action,
        string? notes,
        string? userName,
        int? seizureRequestId = null)
    {
        return new StatusHistory
        {
            EntityType = entityType,
            EntityId = entityId,
            SeizureRequestId = seizureRequestId,
            FromStatusId = fromStatusId,
            ToStatusId = toStatusId,
            Action = action,
            Notes = notes,
            UserName = userName
        };
    }

    public static int AllocationStatus(decimal quantity, decimal remainingQuantity)
    {
        if (remainingQuantity <= 0)
            return (int)SeizureStatus.FullyAllocated;
        if (remainingQuantity < quantity)
            return (int)SeizureStatus.PartiallyAllocated;
        return (int)SeizureStatus.SalesEntity;
    }

    public static bool TryForward(int fromStatusId, int toStatusId, out string? error)
    {
        error = null;
        var allowed = (fromStatusId, toStatusId) switch
        {
            ((int)SeizureStatus.Registration, (int)SeizureStatus.SubCommittee) => true,
            ((int)SeizureStatus.SubCommittee, (int)SeizureStatus.PermanentCommittee) => true,
            ((int)SeizureStatus.PermanentCommittee, (int)SeizureStatus.SalesEntity) => true,
            ((int)SeizureStatus.PermanentCommittee, (int)SeizureStatus.ServicesAgency) => true,
            ((int)SeizureStatus.PermanentCommittee, (int)SeizureStatus.PartiallyAllocated) => true,
            ((int)SeizureStatus.PermanentCommittee, (int)SeizureStatus.FullyAllocated) => true,
            ((int)SeizureStatus.SalesEntity, (int)SeizureStatus.ServicesAgency) => true,
            ((int)SeizureStatus.SalesEntity, (int)SeizureStatus.PartiallyAllocated) => true,
            ((int)SeizureStatus.SalesEntity, (int)SeizureStatus.FullyAllocated) => true,
            ((int)SeizureStatus.SalesEntity, (int)SeizureStatus.FinanceAuthority) => true,
            ((int)SeizureStatus.ServicesAgency, (int)SeizureStatus.FinanceAuthority) => true,
            ((int)SeizureStatus.PartiallyAllocated, (int)SeizureStatus.ServicesAgency) => true,
            ((int)SeizureStatus.PartiallyAllocated, (int)SeizureStatus.FullyAllocated) => true,
            ((int)SeizureStatus.PartiallyAllocated, (int)SeizureStatus.FinanceAuthority) => true,
            ((int)SeizureStatus.FullyAllocated, (int)SeizureStatus.ServicesAgency) => true,
            ((int)SeizureStatus.FullyAllocated, (int)SeizureStatus.FinanceAuthority) => true,
            ((int)SeizureStatus.FinanceAuthority, (int)SeizureStatus.Completed) => true,
            _ => false
        };

        if (allowed)
            return true;

        error = "انتقال الحالة غير مسموح";
        return false;
    }

    public static bool TryReturn(int fromStatusId, int toStatusId, out string? error)
    {
        error = null;
        var allowed = (fromStatusId, toStatusId) switch
        {
            ((int)SeizureStatus.SubCommittee, (int)SeizureStatus.Registration) => true,
            ((int)SeizureStatus.PermanentCommittee, (int)SeizureStatus.SubCommittee) => true,
            ((int)SeizureStatus.SalesEntity, (int)SeizureStatus.PermanentCommittee) => true,
            ((int)SeizureStatus.ServicesAgency, (int)SeizureStatus.PermanentCommittee) => true,
            ((int)SeizureStatus.FinanceAuthority, (int)SeizureStatus.Registration) => true,
            ((int)SeizureStatus.FinanceAuthority, (int)SeizureStatus.SubCommittee) => true,
            ((int)SeizureStatus.FinanceAuthority, (int)SeizureStatus.SalesEntity) => true,
            ((int)SeizureStatus.ServicesAgency, (int)SeizureStatus.ServicesAgency) => true,
            ((int)SeizureStatus.FinanceAuthority, (int)SeizureStatus.ServicesAgency) => true,
            _ => false
        };

        if (allowed)
            return true;

        error = "لا يمكن إرجاع الضبطية في هذه الحالة";
        return false;
    }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

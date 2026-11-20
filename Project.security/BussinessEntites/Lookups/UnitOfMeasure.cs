namespace Project.Security;

public class UnitOfMeasure : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<SeizureRequestDetail>? SeizureRequestDetails { get; set; }
}

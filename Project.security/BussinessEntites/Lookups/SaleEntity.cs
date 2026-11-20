namespace Project.Security;

public class SaleEntity : BaseEntity
{
    public string Name { get; set; }
    public bool IsPublicService { get; set; } = false;
    public ICollection<SaleLot>? SaleLots { get; set; }
}

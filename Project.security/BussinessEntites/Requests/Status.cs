namespace Project.Security;

public class Status : BaseEntity
{
    public string Name { get; set; }
    public ICollection<SeizureRequestDetail>? SeizureRequestDetails { get; set; }

}
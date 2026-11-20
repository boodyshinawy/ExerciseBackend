namespace Project.Security;

public class Item : BaseEntity
{
    public string Name { get; set; }
    public int ItemCategoryId { get; set; }
    public ItemCategory? ItemCategory { get; set; }
    public ICollection<SeizureRequestDetail>? SeizureRequestDetails { get; set; }


}

namespace Project.Security;

public class ItemCategory : BaseEntity
{
    public string Name { get; set; }
    public string Color { get; set; }
    public ICollection<Item>? Items { get; set; }

}

namespace Project.Api;

public class ItemDto : BaseDto
{
    public string Name { get; set; }
    public int ItemCategoryId { get; set; }


}

public class ItemWithCategory : BaseDto
{
    public string Name { get; set; }
    public ItemCategoryDto Catgory { get; set; }

}

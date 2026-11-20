
namespace Project.Api;

public class CategoryWithItems
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Items { get; set; }
}

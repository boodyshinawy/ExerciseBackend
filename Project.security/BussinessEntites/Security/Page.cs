using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Security;

public class Page :BaseAuditEntity
{
    public string? Label { get; set; }
    public string? To { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int SortNumber { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsNavigable { get; set; }
    public bool IsSidebarItem { get; set; }

    [NotMapped]
    public string FullPath { get; set; }

    public virtual Page? PageParent  { get; set; }
    public virtual ICollection<Page>? ChildPages { get; set; }
    public List<PageRole>? PageRoles { get; set; }
    
}

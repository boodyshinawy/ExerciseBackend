using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Project.Security;

public class BaseEntity
{
    public int Id { get; set; }
    [NotMapped]
    public DateTime TimeStamp { get; set; }
    public bool IsDeleted { get; set; }

}

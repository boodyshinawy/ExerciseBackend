using System;
using System.Collections.Generic;

namespace Project.Security;

public partial class Firm
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? CreatedDate { get; set; }
    public bool IsDeleted { get; set; }
}

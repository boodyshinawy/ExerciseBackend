namespace Project.Security;

public class ApplicationUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? SecurityStamp { get; set; }

    public string? RankName { get; set; }

    public string Name { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public string Status { get; set; } = "Active";

    public DateTime RefreshTokenExpiryTime { get; set; }

    public bool IsDeleted { get; set; }

    public string FirmId { get; set; } = null!;

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public Firm? Firm { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

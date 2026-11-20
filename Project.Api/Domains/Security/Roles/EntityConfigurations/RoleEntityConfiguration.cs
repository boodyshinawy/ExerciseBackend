namespace Project.Api;

public class RoleEntityConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasMaxLength(450);
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.NameAr).HasMaxLength(256);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(e => e.Color).HasMaxLength(100).IsRequired();
        builder.Property(e => e.RoleMember);

        builder.HasIndex(r => r.Name).IsUnique();
    }
}

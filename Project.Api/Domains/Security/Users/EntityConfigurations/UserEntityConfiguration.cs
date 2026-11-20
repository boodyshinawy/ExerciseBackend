namespace Project.Api;

public class UserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasMaxLength(450);
        builder.Property(e => e.UserName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(256).IsRequired();
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.HasIndex(e => e.IsDeleted);
        builder.Property(e => e.Status).HasMaxLength(100);
        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.HasOne(e => e.Firm).WithMany().HasForeignKey(e => e.FirmId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

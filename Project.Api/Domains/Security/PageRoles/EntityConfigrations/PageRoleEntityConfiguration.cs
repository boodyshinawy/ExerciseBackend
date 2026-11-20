namespace Project.Api;

public class PageRoleEntityConfiguration : BaseEntityConfiguration<PageRole>
{
    public override void Configure(EntityTypeBuilder<PageRole> builder)
    {
        builder.HasKey(e => new { e.RoleId, e.PageId });

        builder.Property(e => e.ReadAction).HasDefaultValue(false);

        builder.Property(e => e.CreateAction).HasDefaultValue(false);

        builder.Property(e => e.UpdateAction).HasDefaultValue(false);

        builder.Property(e => e.DeleteAction).HasDefaultValue(false);


        builder.HasIndex(e => new { e.RoleId, e.PageId }).HasDatabaseName("RolePageIndex").IsUnique();

        builder.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).HasConstraintName("FK_PageRoleRole").OnDelete(DeleteBehavior.Restrict);


        base.Configure(builder);

    }
}
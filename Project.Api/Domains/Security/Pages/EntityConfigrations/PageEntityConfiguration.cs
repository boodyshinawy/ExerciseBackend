namespace Project.Api;

public class PageEntityConfiguration : BaseAuditEntityConfiguration<Page>
{
    public override void Configure(EntityTypeBuilder<Page> builder)
    {

        builder.HasIndex(e => e.SortNumber)
            .HasDatabaseName("ix_page_sort_number")
            .IsUnique()
            .HasFilter("parent_id IS NULL");

        builder.HasIndex(e => new { e.ParentId, e.SortNumber })
            .HasDatabaseName("ix_page_parent_id_sort_number")
            .IsUnique()
            .HasFilter("parent_id IS NOT NULL");

        builder.Property(e => e.Label).HasMaxLength(100)
               .IsRequired();

        builder.Property(e => e.Icon).HasMaxLength(100)
               .IsRequired();

        builder.Property(e => e.To).HasMaxLength(50);


        builder.Property(e => e.SortNumber).HasPrecision(3)
               .IsRequired();

        builder.HasMany(e => e.ChildPages).WithOne(e => e.PageParent).HasForeignKey(e => e.ParentId)
            .HasConstraintName("FK_PageChildPage").OnDelete(DeleteBehavior.SetNull); ;
        builder.HasMany(e => e.PageRoles).WithOne(e => e.Page).HasForeignKey(e => e.PageId)
            .HasConstraintName("FK_PagePageRole").OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
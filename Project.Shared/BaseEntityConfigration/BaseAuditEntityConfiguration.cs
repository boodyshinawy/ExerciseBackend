namespace Project.Shared;

public class BaseAuditEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity>, IEntityTypeConfiguration<TEntity>
    where TEntity : BaseAuditEntity
{
    public new virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.CreatedBy);

        builder.Property(e => e.CreatedDate);

        builder.Property(e => e.NewState)
               .HasPrecision(2);

        builder.Property(e => e.OldState)
               .HasPrecision(2);

        builder.Property(e => e.LastModifiedDate);

        builder.Property(e => e.LastModifiedBy);

    }
}

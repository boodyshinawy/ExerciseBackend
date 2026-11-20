namespace Project.Shared;

public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Ignore(e => e.TimeStamp);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.HasIndex(e => e.IsDeleted).HasDatabaseName($"IsDeleted{typeof(TEntity).Name}Index");
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.HasQueryFilter(e=>!e.IsDeleted);
    }
}

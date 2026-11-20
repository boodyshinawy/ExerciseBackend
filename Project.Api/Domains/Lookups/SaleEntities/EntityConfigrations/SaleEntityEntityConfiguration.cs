namespace Project.Api;

public class SaleEntityEntityConfiguration : BaseEntityConfiguration<SaleEntity>
{
    public override void Configure(EntityTypeBuilder<SaleEntity> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.HasMany(e => e.SaleLots).WithOne(e => e.SaleEntity).HasForeignKey(e => e.SaleEntityId)
            .OnDelete(DeleteBehavior.Restrict);
        base.Configure(builder);
    }
}

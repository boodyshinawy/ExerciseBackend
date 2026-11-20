namespace Project.Api;

public class UnitOfMeasureEntityConfiguration : BaseEntityConfiguration<UnitOfMeasure>
{
    public override void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.Property(e => e.Name)
               .HasMaxLength(100)
               .IsRequired();

        base.Configure(builder);
    }
}

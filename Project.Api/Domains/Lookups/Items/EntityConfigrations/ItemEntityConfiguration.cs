namespace Project.Api;

public class ItemEntityConfiguration : BaseEntityConfiguration<Item>
{
    public override void Configure(EntityTypeBuilder<Item> builder)
    {


        builder.Property(e => e.Name).HasMaxLength(100)
               .IsRequired();
        builder.HasMany(e => e.SeizureRequestDetails).WithOne(e => e.Item).HasForeignKey(e => e.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}
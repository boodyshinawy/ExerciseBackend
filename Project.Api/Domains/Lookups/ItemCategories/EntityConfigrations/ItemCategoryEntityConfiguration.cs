namespace Project.Api;

public class ItemCategoryEntityConfiguration : BaseEntityConfiguration<ItemCategory>
{
    public override void Configure(EntityTypeBuilder<ItemCategory> builder)
    {


        builder.Property(e => e.Name).HasMaxLength(100)
               .IsRequired();
        builder.Property(e => e.Color).HasMaxLength(100)
       .HasDefaultValue("success");
        builder.HasMany(e => e.Items).WithOne(e => e.ItemCategory).HasForeignKey(e => e.ItemCategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        base.Configure(builder);
    }
}
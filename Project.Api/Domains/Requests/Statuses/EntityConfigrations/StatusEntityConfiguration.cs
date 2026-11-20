namespace Project.Api;

public class StatusEntityConfiguration : BaseEntityConfiguration<Status>
{
    public override void Configure(EntityTypeBuilder<Status> builder)
    {


        builder.Property(e => e.Name).HasMaxLength(100)
               .IsRequired();

        base.Configure(builder);
    }
}
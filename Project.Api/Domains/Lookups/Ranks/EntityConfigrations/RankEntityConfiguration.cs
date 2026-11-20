namespace Project.Api;

public class RankEntityConfiguration : BaseEntityConfiguration<Rank>
{
    public override void Configure(EntityTypeBuilder<Rank> builder)
    {


        builder.Property(e => e.Name).HasMaxLength(100)
               .IsRequired();
        base.Configure(builder);
    }
}
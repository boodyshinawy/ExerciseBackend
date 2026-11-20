namespace Project.Api;

public class StatusHistoryEntityConfiguration : BaseAuditEntityConfiguration<StatusHistory>
{
    public override void Configure(EntityTypeBuilder<StatusHistory> builder)
    {
        builder.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Action).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.Property(e => e.UserName).HasMaxLength(200);
        builder.HasIndex(e => new { e.EntityType, e.EntityId });
        builder.HasIndex(e => e.SeizureRequestId);
        base.Configure(builder);
    }
}

namespace Project.Api;

public class SeizureRequestDetailEntityConfiguration : BaseAuditEntityConfiguration<SeizureRequestDetail>
{
    public override void Configure(EntityTypeBuilder<SeizureRequestDetail> builder)
    {
        builder.Property(e => e.Quantity).HasDefaultValue(1);
        builder.Property(e => e.RemainingQuantity).HasDefaultValue(0);
        builder.ToTable(t => t.HasCheckConstraint(
            "ck_seizure_detail_remaining_qty",
            "remaining_quantity >= 0 AND remaining_quantity <= quantity"));

        builder.HasOne(e => e.Status).WithMany(e => e.SeizureRequestDetails).HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UnitOfMeasure).WithMany(e => e.SeizureRequestDetails).HasForeignKey(e => e.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        // time_stamp column removed from this table (created_date is used instead)
        builder.Ignore(e => e.TimeStamp);

        base.Configure(builder);
    }
}

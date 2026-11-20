namespace Project.Api;

public class SaleLotEntityConfiguration : BaseAuditEntityConfiguration<SaleLot>
{
    public override void Configure(EntityTypeBuilder<SaleLot> builder)
    {
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.Property(e => e.RejectionReason).HasMaxLength(2000);
        builder.Property(e => e.DepositReceiptPath).HasMaxLength(1000);
        builder.Property(e => e.TotalAmountCollected).HasColumnType("numeric(18,2)");
        builder.Property(e => e.StatusId).IsRequired();
        builder.HasMany(e => e.LotItemAllocations).WithOne(e => e.SaleLot).HasForeignKey(e => e.SaleLotId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(e => e.FinAccepetedDate).HasColumnName("fin_accepeted_date");
        // time_stamp column removed from this table (created_date is used instead)
        builder.Ignore(e => e.TimeStamp);
        base.Configure(builder);
    }
}

namespace Project.Api;

public class LotItemAllocationEntityConfiguration : BaseAuditEntityConfiguration<LotItemAllocation>
{
    public override void Configure(EntityTypeBuilder<LotItemAllocation> builder)
    {
        builder.Property(e => e.AllocatedQuantity).IsRequired();
        builder.HasIndex(e => new { e.SaleLotId, e.SeizureRequestDetailId }).IsUnique();
        builder.HasOne(e => e.SeizureRequestDetail).WithMany(e => e.LotItemAllocations)
            .HasForeignKey(e => e.SeizureRequestDetailId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("ck_lot_item_allocation_qty", "allocated_quantity > 0"));
        // time_stamp column removed from this table (created_date is used instead)
        builder.Ignore(e => e.TimeStamp);
        base.Configure(builder);
    }
}

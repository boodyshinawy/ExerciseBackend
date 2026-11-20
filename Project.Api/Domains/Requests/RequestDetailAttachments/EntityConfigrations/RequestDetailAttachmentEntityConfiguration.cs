namespace Project.Api;

public class RequestDetailAttachmentEntityConfiguration : BaseAuditEntityConfiguration<RequestDetailAttachment>
{
    public override void Configure(EntityTypeBuilder<RequestDetailAttachment> builder)
    {
        builder.HasOne(e => e.SeizureRequestDetail).WithMany(e => e.RequestDetailAttachments).HasForeignKey(e => e.SeizureRequestDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        base.Configure(builder);
    }
}

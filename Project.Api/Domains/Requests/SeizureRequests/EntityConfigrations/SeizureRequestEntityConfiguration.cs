namespace Project.Api;

public class SeizureRequestEntityConfiguration : BaseAuditEntityConfiguration<SeizureRequest>
{
    public override void Configure(EntityTypeBuilder<SeizureRequest> builder)
    {
        builder.HasMany(e => e.SeizureRequestDetails).WithOne(e => e.SeizureRequest).HasForeignKey(e => e.SeizureRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Firm).WithMany().HasForeignKey(e => e.FirmId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.TechnicalReport).HasMaxLength(4000);
        builder.Property(e => e.SubCommitteeOpinion).HasMaxLength(2000);
        builder.Property(e => e.CaseNumber).HasMaxLength(100);
        builder.Property(e => e.MahdrNumber).HasMaxLength(100);

        // Return reason fields
        builder.Property(e => e.ReturnReasonFromSubCommite).HasMaxLength(2000);
        builder.Property(e => e.ReturnReasonFromPermanentCommite).HasMaxLength(2000);

        // Amana accept fields
        builder.Property(e => e.AmanaAcceptNumber).HasMaxLength(100).HasColumnName("amana_accept_number");
        builder.Property(e => e.AmanaAcceptDate).HasColumnName("amana_accept_date");

        // Sub-Committee session fields
        builder.Property(e => e.SubCommitteeNumber).HasMaxLength(100).HasColumnName("sub_committee_number");
        builder.Property(e => e.SubCommitteeDate).HasColumnName("sub_committee_date");

        // Permanent Committee session fields
        builder.Property(e => e.PermanentCommitteeNumber).HasMaxLength(100).HasColumnName("permanent_committee_number");
        builder.Property(e => e.PermanentCommitteeDate).HasColumnName("permanent_committee_date");

        // time_stamp column removed from this table (created_date is used instead)
        builder.Ignore(e => e.TimeStamp);

        base.Configure(builder);
    }
}


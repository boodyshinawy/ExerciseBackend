namespace Project.Security;

public class SeizureRequest : BaseAuditEntity
{
    public string FirmId { get; set; }
    public DateTime RequestDate { get; set; }
    public string? TechnicalReport { get; set; }
    public string? SubCommitteeOpinion { get; set; }

    // New business fields
    public string? CaseNumber { get; set; }
    public string? MahdrNumber { get; set; }

    // Return reasons from committees
    public string? ReturnReasonFromSubCommite { get; set; }
    public string? ReturnReasonFromPermanentCommite { get; set; }

    // Permanent Committee (Amana) approval fields
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDate { get; set; }

    // Sub-Committee session fields
    public string? SubCommitteeNumber { get; set; }
    public DateTime? SubCommitteeDate { get; set; }

    // Permanent Committee session fields
    public string? PermanentCommitteeNumber { get; set; }
    public DateTime? PermanentCommitteeDate { get; set; }

    public ICollection<SeizureRequestDetail>? SeizureRequestDetails { get; set; }
    public Firm? Firm { get; set; }

}

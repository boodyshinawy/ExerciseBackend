namespace Project.Api;

public class SeizureRequestDto : BaseDto
{
    public string? FirmId { get; set; }
    public string? FirmName { get; set; }
    public DateTime RequestDate { get; set; }
    public string? CaseNumber { get; set; }
    public string? MahdrNumber { get; set; }
    public string? ReturnReasonFromSubCommite { get; set; }
    public string? ReturnReasonFromPermanentCommite { get; set; }
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDate { get; set; }
    public string? SubCommitteeNumber { get; set; }
    public DateTime? SubCommitteeDate { get; set; }
    public string? PermanentCommitteeNumber { get; set; }
    public DateTime? PermanentCommitteeDate { get; set; }
}

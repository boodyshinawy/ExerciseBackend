namespace Project.Api;

public class SeizureRequestReadDto : BaseDto
{
    public string FirmId { get; set; }
    public string? FirmName { get; set; }
    public DateTime RequestDate { get; set; }
    public int HasSeizuresDetail { get; set; }
    public int SeizuresDone { get; set; }
    public int SeizuresPending { get; set; }
    public int SeizuresRevision { get; set; }
    public int? StatusId { get; set; }
    public string? StatusName { get; set; }
    public string? TechnicalReport { get; set; }
    public string? SubCommitteeOpinion { get; set; }
    public bool IsLocked { get; set; }
    public List<string>? RoleMembers { get; set; }
    public bool HasPendingRequests { get; set; }
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
public class SeizureReadDto
{
    public List<SeizureRequestReadDto>? SeizureRequests { get; set; }

    public List<string>? RoleMembers { get; set; }
}

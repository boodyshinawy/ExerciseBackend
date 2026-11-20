namespace Project.Api;

public class DashboardData
{
    public int EditingSeizures { get; set; }
    public int PendingSeizures { get; set; }
    public int AcceptedSeizures { get; set; }
    public List<SeizureRequestDetailReadDto> Seizures { get; set; }
    public List<string>? RoleMembers { get; set; }

}

public class DonotusData
{
    public int StatusId { get; set; }
    public int SeizuresCount { get; set; }
}
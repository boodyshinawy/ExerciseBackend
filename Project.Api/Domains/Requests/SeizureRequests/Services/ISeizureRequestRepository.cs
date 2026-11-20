


namespace Project.Api;

public interface ISeizureRequestRepository : IBaseRepository<SeizureRequest>
{
    Task ChangeSeizuresUnderRequestToSubCommittee(int id, string? userName);
    Task<List<FirmDto>> GetFirms();
    Task<List<SeizureRequestReadDto>> GetSeizureRequests(bool all, string firmCode, List<string> roleMembers);
    Task<List<WorkflowSeizureDto>> GetSeizuresByStatus(int statusId);
    Task<List<WorkflowSeizureDto>> GetSeizuresByStatusScoped(int statusId, string? firmCode, bool isGlobal);
    Task BulkSubmitToSubCommittee(List<int> seizureRequestIds, string? firmCode, string? userName);
    Task ReturnSeizure(ReturnSeizureDto dto, string? userName);
    Task ForwardSeizure(ForwardSeizureDto dto, int targetStatusId, string? userName);
    Task BulkApprove(BulkApproveDto dto, string? userName);
    Task<bool> HasImmutableDetails(int requestId);
}
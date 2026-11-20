

namespace Project.Api;

public interface ISeizureRequestService : IBaseService<SeizureRequest, SeizureRequestDto>
{
    Task<OperationResult<string>> ChangeSeizuresUnderRequestToSubCommittee(int id, string? userName);
    Task<OperationResult<List<FirmDto>>> GetFirms();
    Task<OperationResult<SeizureReadDto>> GetSeizureRequests(bool all, string firmCode, List<string> roleMembers);
    Task<OperationResult<List<WorkflowSeizureDto>>> GetSeizuresByStatus(int statusId);
    Task<OperationResult<List<WorkflowSeizureDto>>> GetSeizuresByStatusScoped(int statusId, string? firmCode, bool isGlobal);
    Task<OperationResult<string>> BulkSubmitToSubCommittee(BulkSubmitToSubCommitteeDto dto, string? firmCode, string? userName);
    Task<OperationResult<string>> ReturnSeizure(ReturnSeizureDto dto, string? userName);
    Task<OperationResult<string>> ForwardToPermanentCommittee(ForwardSeizureDto dto, string? userName);
    Task<OperationResult<string>> BulkApprove(BulkApproveDto dto, string? userName);
}

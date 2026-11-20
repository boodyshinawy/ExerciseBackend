

namespace Project.Api;

public interface ISeizureRequestDetailService : IBaseService<SeizureRequestDetail, SeizureRequestDetailDto>
{
    Task<OperationResult<string>> ChangeRequestStatus(ChangeRequestStatus changeRequestStatus, string? userName);
    Task<OperationResult<SeizureRequestDetailReadDto>> CreateRequestDetailForRequest(SeizureRequestDetailDto seizureRequestDetailDto, List<string> roleMembers);
    Task<OperationResult<string>> FinanceAction(int id, FinanceAction financeAction, bool financeDecision);
    Task<OperationResult<byte[]>> GeneratePdf(string firmId, SearchParams searchParams, List<string> roleMembers);
    Task<OperationResult<DashboardData>> GetDashboardData(string firmId, List<string> roleMembers);
    Task<OperationResult<LookupsForRequest>> GetLookupsForRequest();
    Task<OperationResult<SeizureDetailDto>> GetRequestDetailForRequest(int seizureRequestId,List<string> roleMembers);
    Task<OperationResult<List<SeizureRequestDetailReadDto>>> SeizuresOnDuration(string firmId, List<string> roleMembers, DateDuration dateDuration);
    Task<OperationResult<SearchReadDto>> SearchReadDto(string firmId, List<string> roleMembers);
    Task<OperationResult<List<SeizureRequestDetailReadDto>>> SearchResult(string firmId, SearchParams searchParams, List<string> roleMembers);
}




using Project.Security;

namespace Project.Api;

public interface ISeizureRequestDetailRepository : IBaseRepository<SeizureRequestDetail>
{
    Task ChangeRequestStatus(ChangeRequestStatus changeRequestStatus, string? userName);
    Task FinanceAction(int id, FinanceAction financeAction, bool financeDecision);
    Task<DashboardData> GetDashboardData(string firmId, List<string> roleMembers);
    Task<LookupsForRequest> GetLookupsForRequest();
    Task<List<SeizureRequestDetailReadDto>> GetRequestDetailForRequest(int seizureRequestId,List<string> roleMembers);
    Task<SeizureRequestDetailReadDto> GetSpecificRequestDetail(int id, List<string> roleMembers);
    Task<List<SeizureRequestDetailReadDto>> SeizuresOnDuration(string firmId, List<string> roleMembers, DateDuration dateDuration);
    Task<SearchReadDto> SearchReadDto(string firmId, List<string> roleMembers);
    Task<List<SeizureRequestDetailReadDto>> SearchResult(string firmId, SearchParams searchParams, List<string> roleMembers);
}
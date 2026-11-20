



namespace Project.Api;

public interface IRequestDetailAttachmentRepository : IBaseRepository<RequestDetailAttachment>
{
    Task CreateRequestDetailAttachmensts(int seizureRequestDetailId, List<RequestDetailAttachmentUploading> requestDetailAttachmentUploadings, string firmCode);
    Task DeleteRequestDetailAttachmensts(int attachemntId);
    Task<List<RequestDetailAttachmentDto>> GetRequestDetailAttachmensts(int seizureRequestDetailId);
}
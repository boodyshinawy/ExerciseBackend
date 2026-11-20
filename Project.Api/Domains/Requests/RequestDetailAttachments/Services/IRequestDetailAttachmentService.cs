


namespace Project.Api;

public interface IRequestDetailAttachmentService : IBaseService<RequestDetailAttachment, RequestDetailAttachmentDto>
{
    Task<OperationResult<string>> DeleteRequestDetailAttachmensts(int attachmentId);
    Task<OperationResult<string>> CreateRequestDetailAttachmensts(int seizureRequestDetailId, List<RequestDetailAttachmentUploading> requestDetailAttachmentUploadings, string firmCode);
    Task<OperationResult<List<RequestDetailAttachmentDto>>> GetRequestDetailAttachmensts(int seizureRequestDetailId);
}

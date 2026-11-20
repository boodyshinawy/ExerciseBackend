using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class RequestDetailAttachmentService : BaseService<RequestDetailAttachment, RequestDetailAttachmentDto>, IRequestDetailAttachmentService
    {
        private readonly IRequestDetailAttachmentRepository _requestDetailAttachmentRepository;
        private readonly ISeizureRequestDetailRepository _seizureRequestDetailRepository;

        public RequestDetailAttachmentService(
            IValidator<RequestDetailAttachment> validator,
            IRequestDetailAttachmentRepository repository,
            ISeizureRequestDetailRepository seizureRequestDetailRepository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _requestDetailAttachmentRepository = repository;
            _seizureRequestDetailRepository = seizureRequestDetailRepository;
        }
        public async Task<OperationResult<List<RequestDetailAttachmentDto>>> GetRequestDetailAttachmensts(int seizureRequestDetailId)
        {
            try
            {
                var attachments = await _requestDetailAttachmentRepository.GetRequestDetailAttachmensts(seizureRequestDetailId);
                return OperationResult<List<RequestDetailAttachmentDto>>.Succeeded("�� ������� �������� �����", attachments);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<List<RequestDetailAttachmentDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<List<RequestDetailAttachmentDto>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }
        public async Task<OperationResult<string>> CreateRequestDetailAttachmensts(int seizureRequestDetailId,List<RequestDetailAttachmentUploading> requestDetailAttachmentUploadings, string firmCode)
        {
            try
            {
                var entity = await _seizureRequestDetailRepository.Get(seizureRequestDetailId);
            if (entity != null && SeizureWorkflow.IsImmutable(entity.StatusId))
                return OperationResult<string>.Failed("لا يمكن تعديل مرفقات المضبوط بعد اعتماد اللجنة الدائمة");

            await _requestDetailAttachmentRepository.CreateRequestDetailAttachmensts(seizureRequestDetailId,requestDetailAttachmentUploadings, firmCode);
                return OperationResult<string>.Succeeded("�� ����� �������� �����");

            }
            catch (RepositoryException ex)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }
        public async Task<OperationResult<string>> DeleteRequestDetailAttachmensts(int attachmentId)
        {
            try
            {
                await _requestDetailAttachmentRepository.DeleteRequestDetailAttachmensts(attachmentId);
                return OperationResult<string>.Succeeded("�� ��� ������ �����");

            }
            catch (RepositoryException ex)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<string>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }


    }


}

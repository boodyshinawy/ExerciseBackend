using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Project.Security;

using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class SeizureRequestDetailService : BaseService<SeizureRequestDetail, SeizureRequestDetailDto>, ISeizureRequestDetailService
    {
        private readonly ISeizureRequestDetailRepository _seizureRequestDetailRepository;
        private readonly ToDocument _toDocument;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public SeizureRequestDetailService(
            IValidator<SeizureRequestDetail> validator,
            ISeizureRequestDetailRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer,
            ToDocument toDocument,
            IWebHostEnvironment env,
            ApplicationDbContext context)
            : base(repository, validator, mapper, localizer)
        {
            _seizureRequestDetailRepository = repository;
            _toDocument = toDocument;
            _env = env;
            _context = context;
        }
        public async Task<OperationResult<SeizureRequestDetailReadDto>> CreateRequestDetailForRequest(SeizureRequestDetailDto seizureRequestDetailDto, List<string> roleMembers)
        {
            try
            {
                var entity = _mapper.Map<SeizureRequestDetail>(seizureRequestDetailDto);
                entity.StatusId = (int)SeizureStatus.Registration;
                entity.RemainingQuantity = entity.Quantity;
                entity.UnitOfMeasureId = seizureRequestDetailDto.UnitOfMeasureId;
                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return OperationResult<SeizureRequestDetailReadDto>.BadRequest(_localizer["ValidationFailed"]);
                }
                var requestDetails = await _seizureRequestDetailRepository.Add(entity);
                var specificSeizure = await _seizureRequestDetailRepository.GetSpecificRequestDetail(entity.Id, roleMembers);
                //List<PagesOnRole>? pageNameWithRoleNameDto = _mapper.Map<List<PagesOnRole>>(pageNameWithRoleName);
                return OperationResult<SeizureRequestDetailReadDto>.Succeeded(_localizer["EntityRetrievedSuccessfully"], specificSeizure);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<SeizureRequestDetailReadDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<SeizureRequestDetailReadDto>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }

        public async Task<OperationResult<SeizureDetailDto>> GetRequestDetailForRequest(int seizureRequestId, List<string> roleMembers)
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.GetRequestDetailForRequest(seizureRequestId, roleMembers);
                var seizureDetail=new SeizureDetailDto();
                seizureDetail.SeizureRequestDetailReadDtos = requestDetails;
                seizureDetail.RoleMembers = roleMembers;
                //List<PagesOnRole>? pageNameWithRoleNameDto = _mapper.Map<List<PagesOnRole>>(pageNameWithRoleName);
                return OperationResult<SeizureDetailDto>.Succeeded(_localizer["EntityRetrievedSuccessfully"], seizureDetail);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<SeizureDetailDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<SeizureDetailDto>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }
        public async Task<OperationResult<SearchReadDto>> SearchReadDto(string firmId, List<string> roleMembers)
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.SearchReadDto(firmId, roleMembers);
                return OperationResult<SearchReadDto>.Succeeded(_localizer["EntityRetrievedSuccessfully"], requestDetails);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<SearchReadDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<SearchReadDto>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }

        public async Task<OperationResult<List<SeizureRequestDetailReadDto>>> SeizuresOnDuration(string firmId, List<string> roleMembers,DateDuration dateDuration)
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.SeizuresOnDuration(firmId, roleMembers,dateDuration);
                return OperationResult<List<SeizureRequestDetailReadDto>>.Succeeded(_localizer["EntityRetrievedSuccessfully"], requestDetails);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<List<SeizureRequestDetailReadDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<List<SeizureRequestDetailReadDto>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }
        public async Task<OperationResult<List<SeizureRequestDetailReadDto>>> SearchResult(string firmId,SearchParams searchParams, List<string> roleMembers)
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.SearchResult(firmId, searchParams, roleMembers);
                return OperationResult<List<SeizureRequestDetailReadDto>>.Succeeded(_localizer["EntityRetrievedSuccessfully"], requestDetails);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<List<SeizureRequestDetailReadDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<List<SeizureRequestDetailReadDto>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }

        public async Task<OperationResult<byte[]>> GeneratePdf(string firmId, SearchParams searchParams, List<string> roleMembers)
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.SearchResult(firmId, searchParams, roleMembers);
                _toDocument.SeizureRequestDetailReadDtos = requestDetails.Where(e=>e.StatusId==(int)SeizureStatus.Completed).ToList();
                _toDocument._imagePath = _env.ContentRootPath;
                var pdf = _toDocument.GeneratePdf();

                return OperationResult<byte[]>.Succeeded(_localizer["EntityRetrievedSuccessfully"], pdf);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<byte[]>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<byte[]>.InternalServerError("��� �� ������� �����");
            }

        }

        public async Task<OperationResult<DashboardData>> GetDashboardData(string firmId,List<string> roleMembers)
        {
            try
            {
                var dashboradCards = await _seizureRequestDetailRepository.GetDashboardData(firmId, roleMembers);
                return OperationResult<DashboardData>.Succeeded(_localizer["EntityRetrievedSuccessfully"], dashboradCards);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<DashboardData>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<DashboardData>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }
        public async Task<OperationResult<LookupsForRequest>> GetLookupsForRequest()
        {
            try
            {
                var requestDetails = await _seizureRequestDetailRepository.GetLookupsForRequest();
                return OperationResult<LookupsForRequest>.Succeeded(_localizer["EntityRetrievedSuccessfully"], requestDetails);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<LookupsForRequest>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<LookupsForRequest>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }

        public async Task<OperationResult<string>> ChangeRequestStatus(ChangeRequestStatus changeRequestStatus, string? userName)
        {
            try
            {
                 await _seizureRequestDetailRepository.ChangeRequestStatus(changeRequestStatus, userName);
                return OperationResult<string>.Succeeded(_localizer["EntityUpdatedSuccessfully"]);

            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
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

        public override async Task<OperationResult<SeizureRequestDetailDto>> Update(int id, SeizureRequestDetailDto entityDto)
        {
            var existing = await _seizureRequestDetailRepository.Get(id);
            if (existing == null)
                return OperationResult<SeizureRequestDetailDto>.NotFound(_localizer["EntityNotFound"]);
            if (SeizureWorkflow.IsImmutable(existing.StatusId))
                return OperationResult<SeizureRequestDetailDto>.Failed("لا يمكن تعديل المضبوط بعد اعتماد اللجنة الدائمة");

            if (existing.StatusId == (int)SeizureStatus.Registration)
            {
                if (entityDto.Quantity <= 0)
                    entityDto.Quantity = existing.Quantity;
                existing.Quantity = entityDto.Quantity;
                existing.RemainingQuantity = entityDto.Quantity;
            }

            existing.ItemId = entityDto.ItemId;
            existing.ItemName = entityDto.ItemName ?? string.Empty;
            existing.UnitOfMeasureId = entityDto.UnitOfMeasureId;
            existing.UnitOfMeasure = null;
            if (entityDto.UnitPrice.HasValue)
            {
                existing.UnitPrice = entityDto.UnitPrice.Value;
            }

            var updatedEntity = await _seizureRequestDetailRepository.Update(existing);
            var updatedDto = _mapper.Map<SeizureRequestDetailDto>(updatedEntity);
            updatedDto.UnitOfMeasureId = updatedEntity.UnitOfMeasureId;

            if (!string.IsNullOrWhiteSpace(entityDto.UnitOfMeasureName))
            {
                updatedDto.UnitOfMeasureName = entityDto.UnitOfMeasureName;
            }
            else if (existing.UnitOfMeasureId.HasValue)
            {
                var uom = await _context.Set<UnitOfMeasure>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == existing.UnitOfMeasureId.Value);
                if (uom != null)
                {
                    updatedDto.UnitOfMeasureName = uom.Name;
                }
            }

            return OperationResult<SeizureRequestDetailDto>.Succeeded(_localizer["EntityUpdatedSuccessfully"], updatedDto);
        }

        public override async Task<OperationResult<SeizureRequestDetail>> Delete(int id, bool physicalDelete = false)
        {
            var existing = await _seizureRequestDetailRepository.Get(id);
            if (existing == null)
                return OperationResult<SeizureRequestDetail>.NotFound(_localizer["EntityNotFound"]);
            if (SeizureWorkflow.IsImmutable(existing.StatusId))
                return OperationResult<SeizureRequestDetail>.Failed("لا يمكن حذف المضبوط بعد اعتماد اللجنة الدائمة");
            return await base.Delete(id, physicalDelete);
        }

        public async Task<OperationResult<string>> FinanceAction(int id, FinanceAction financeAction,bool financeDecision)
        {
            try
            {
                await _seizureRequestDetailRepository.FinanceAction(id, financeAction, financeDecision);
                return OperationResult<string>.Succeeded(_localizer["EntityUpdatedSuccessfully"]);

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

using Microsoft.Extensions.Localization;

namespace Project.Api
{
    public class SeizureRequestService : BaseService<SeizureRequest, SeizureRequestDto>, ISeizureRequestService
    {
        private readonly ISeizureRequestRepository _seizureRequestRepository;

        public SeizureRequestService(
            IValidator<SeizureRequest> validator,
            ISeizureRequestRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _seizureRequestRepository = repository;
        }

        public async Task<OperationResult<List<FirmDto>>> GetFirms()
        {
            try
            {
                var firms = await _seizureRequestRepository.GetFirms();
                return OperationResult<List<FirmDto>>.Succeeded(_localizer["GetCategoriesWithItems"], firms);
            }
            catch (RepositoryException)
            {
                return OperationResult<List<FirmDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception)
            {
                return OperationResult<List<FirmDto>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }
        }

        public async Task<OperationResult<string>> ChangeSeizuresUnderRequestToSubCommittee(int id, string? userName)
        {
            try
            {
                await _seizureRequestRepository.ChangeSeizuresUnderRequestToSubCommittee(id, userName);
                return OperationResult<string>.Succeeded("تم إرسال المضبوطات إلى اللجنة الفرعية");
            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
            }
            catch (RepositoryException)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception)
            {
                return OperationResult<string>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }
        }

        public async Task<OperationResult<SeizureReadDto>> GetSeizureRequests(bool all, string firmCode, List<string> roleMembers)
        {
            try
            {
                var seizureRequests = await _seizureRequestRepository.GetSeizureRequests(all, firmCode, roleMembers);
                var seizureDto = new SeizureReadDto
                {
                    SeizureRequests = seizureRequests,
                    RoleMembers = roleMembers
                };
                return OperationResult<SeizureReadDto>.Succeeded(_localizer["GetCategoriesWithItems"], seizureDto);
            }
            catch (RepositoryException)
            {
                return OperationResult<SeizureReadDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception)
            {
                return OperationResult<SeizureReadDto>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }
        }

        public async Task<OperationResult<List<WorkflowSeizureDto>>> GetSeizuresByStatus(int statusId)
        {
            try
            {
                var seizures = await _seizureRequestRepository.GetSeizuresByStatus(statusId);
                return OperationResult<List<WorkflowSeizureDto>>.Succeeded("تم استرجاع الضبطيات", seizures);
            }
            catch (Exception)
            {
                return OperationResult<List<WorkflowSeizureDto>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public async Task<OperationResult<List<WorkflowSeizureDto>>> GetSeizuresByStatusScoped(int statusId, string? firmCode, bool isGlobal)
        {
            try
            {
                var seizures = await _seizureRequestRepository.GetSeizuresByStatusScoped(statusId, firmCode, isGlobal);
                return OperationResult<List<WorkflowSeizureDto>>.Succeeded("تم استرجاع الضبطيات", seizures);
            }
            catch (Exception)
            {
                return OperationResult<List<WorkflowSeizureDto>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public async Task<OperationResult<string>> BulkSubmitToSubCommittee(BulkSubmitToSubCommitteeDto dto, string? firmCode, string? userName)
        {
            try
            {
                await _seizureRequestRepository.BulkSubmitToSubCommittee(dto.SeizureRequestIds, firmCode, userName);
                return OperationResult<string>.Succeeded("تم إرسال الضبطيات إلى اللجنة الفرعية بنجاح");
            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public async Task<OperationResult<string>> ReturnSeizure(ReturnSeizureDto dto, string? userName)
        {
            try
            {
                await _seizureRequestRepository.ReturnSeizure(dto, userName);
                return OperationResult<string>.Succeeded("تم إرجاع الضبطية مع تسجيل السبب");
            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public async Task<OperationResult<string>> ForwardToPermanentCommittee(ForwardSeizureDto dto, string? userName)
        {
            try
            {
                await _seizureRequestRepository.ForwardSeizure(dto, (int)SeizureStatus.PermanentCommittee, userName);
                return OperationResult<string>.Succeeded("تم إرسال الضبطية إلى اللجنة الدائمة");
            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public async Task<OperationResult<string>> BulkApprove(BulkApproveDto dto, string? userName)
        {
            try
            {
                await _seizureRequestRepository.BulkApprove(dto, userName);
                return OperationResult<string>.Succeeded("تم اعتماد الضبطيات من اللجنة الدائمة وأصبحت غير قابلة للتعديل");
            }
            catch (DomainException ex)
            {
                return OperationResult<string>.Failed(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public override async Task<OperationResult<SeizureRequestDto>> Create(SeizureRequestDto entityDto)
        {
            try
            {
                entityDto.RequestDate = DateTime.SpecifyKind(entityDto.RequestDate, DateTimeKind.Utc);
                var entity = _mapper.Map<SeizureRequest>(entityDto);
                entity.RequestDate = DateTime.SpecifyKind(entity.RequestDate, DateTimeKind.Utc);

                if (string.IsNullOrWhiteSpace(entity.FirmId))
                {
                    return OperationResult<SeizureRequestDto>.Failed("يجب تحديد الوحدة التابع لها المستخدم");
                }

                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return OperationResult<SeizureRequestDto>.BadRequest(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? _localizer["ValidationFailed"]);
                }

                entity = await _seizureRequestRepository.Add(entity);
                var entityDtoBack = _mapper.Map<SeizureRequestDto>(entity);
                
                var firms = await _seizureRequestRepository.GetFirms();
                entityDtoBack.FirmName = firms.FirstOrDefault(f => f.FirmId == entity.FirmId)?.FirmName ?? string.Empty;

                return OperationResult<SeizureRequestDto>.Succeeded(_localizer["EntityCreatedSuccessfully"], entityDtoBack);
            }
            catch (DomainException ex)
            {
                return OperationResult<SeizureRequestDto>.Failed(ex.Message);
            }
            catch (RepositoryException ex)
            {
                Log.Error(ex, "RepositoryException in Create SeizureRequest");
                return OperationResult<SeizureRequestDto>.Failed(ex.InnerException?.Message ?? _localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in Create SeizureRequest");
                return OperationResult<SeizureRequestDto>.Failed(ex.Message);
            }
        }

        public override async Task<OperationResult<SeizureRequestDto>> Update(int id, SeizureRequestDto entityDto)
        {
            if (await _seizureRequestRepository.HasImmutableDetails(id))
                return OperationResult<SeizureRequestDto>.Failed("لا يمكن تعديل الضبطية بعد اعتماد اللجنة الدائمة");
            entityDto.RequestDate = DateTime.SpecifyKind(entityDto.RequestDate, DateTimeKind.Utc);

            var entity = _mapper.Map<SeizureRequest>(entityDto);
            entity.Id = id;
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                return OperationResult<SeizureRequestDto>.BadRequest(validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? _localizer["ValidationFailed"]);
            }

            var result = await base.Update(id, entityDto);
            if (result.Success && result.Data != null)
            {
                var firms = await _seizureRequestRepository.GetFirms();
                result.Data.FirmName = firms.FirstOrDefault(f => f.FirmId == result.Data.FirmId)?.FirmName ?? string.Empty;
            }
            return result;
        }

        public override async Task<OperationResult<SeizureRequest>> Delete(int id, bool physicalDelete = false)
        {
            if (await _seizureRequestRepository.HasImmutableDetails(id))
                return OperationResult<SeizureRequest>.Failed("لا يمكن حذف الضبطية بعد اعتماد اللجنة الدائمة");
            return await base.Delete(id, physicalDelete);
        }
    }
}

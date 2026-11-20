using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Project.Shared;

namespace Project.Shared
{
    public abstract class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        private readonly IBaseRepository<TEntity> _repository;
        protected readonly IValidator<TEntity> _validator;
        protected readonly IMapper _mapper;
        protected readonly IStringLocalizer<SharedService> _localizer;

        public BaseService(IBaseRepository<TEntity> repository ,IValidator<TEntity> validator, IMapper mapper, IStringLocalizer<SharedService> localizer)
        {

            _validator = validator;
            _mapper = mapper;
            _localizer = localizer;
            _repository = repository;
        }

        public virtual async Task<OperationResult<TDto>> Create(TDto entityDto)
        {
            try
            {
                //CultureInfo culture =Thread.CurrentThread.CurrentCulture;
                //var language = culture.TwoLetterISOLanguageName.ToLower();
                //var localizedValidationFailedMessage = _localizer[SharedServiceKeys.ValidationFailed];

                var entity = _mapper.Map<TEntity>(entityDto);
                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return OperationResult<TDto>.BadRequest(_localizer["ValidationFailed"]);
                }

                entity = await _repository.Add(entity);
                var entityDtoBack =_mapper.Map<TDto>(entity);
                return OperationResult<TDto>.Succeeded(_localizer["EntityCreatedSuccessfully"], entityDtoBack);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<IEnumerable<TEntity>>> Create(IEnumerable<TDto> dtoEntities)
        {
            try
            {
                var entities = _mapper.Map<IEnumerable<TEntity>>(dtoEntities);
                var validationResults = new List<string>();

                foreach (var entity in entities)
                {
                    var validationResult = await _validator.ValidateAsync(entity);
                    if (!validationResult.IsValid)
                    {
                        validationResults.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
                    }
                }

                if (validationResults.Any())
                {
                    //return OperationResult<IEnumerable<TEntity>>.BadRequest(_localizer["ValidationFailed", string.Join(", ", validationResults)]);      
                    return OperationResult<IEnumerable<TEntity>>.BadRequest(_localizer["ValidationFailed"]);
                }

                await _repository.Add(entities);
                return OperationResult<IEnumerable<TEntity>>.Succeeded(_localizer["EntitiesCreatedSuccessfully"], entities);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TDto>> Read(int id)
        {
            try
            {
                var entity = await _repository.Get(id);
                if (entity == null)
                {
                    return OperationResult<TDto>.NotFound(_localizer["EntityNotFound"]);
                }

                var dto = _mapper.Map<TDto>(entity);
                return OperationResult<TDto>.Succeeded(_localizer["EntityRetrievedSuccessfully"], dto);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<IEnumerable<TDto>>> Read()
        {
            try
            {
                var entities = await _repository.Get();
                var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
                return OperationResult<IEnumerable<TDto>>.Succeeded(_localizer["EntitiesRetrievedSuccessfully"], dtos);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<IEnumerable<TDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TDto>>.InternalServerError(_localizer["UnexpectedError", ex.Message]);
            }
        }

        public virtual async Task<OperationResult<IEnumerable<TDto>>> Read(int page, int pageSize)
        {
            try
            {
                var entities = await _repository.Get(page, pageSize);
                var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
                return OperationResult<IEnumerable<TDto>>.Succeeded(_localizer["PagedEntitiesRetrievedSuccessfully"], dtos);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<IEnumerable<TDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TDto>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TDto>> Update(int id, TDto entityDto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(entityDto);

                var updatedEntity = await _repository.Update(id, entity);
                var updatedDto=_mapper.Map<TDto>(updatedEntity);
                return OperationResult<TDto>.Succeeded(_localizer["EntityUpdatedSuccessfully"], updatedDto);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TEntity>> Update(TEntity entity)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return OperationResult<TEntity>.BadRequest(_localizer["ValidationFailed", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))]);
                }

                var updatedEntity = await _repository.Update(entity);
                return OperationResult<TEntity>.Succeeded(_localizer["EntityUpdatedSuccessfully"], updatedEntity);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TDto>> UpdateWithDto(TDto dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);
                var validationResult = await _validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return OperationResult<TDto>.BadRequest(_localizer["ValidationFailed", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))]);
                }

                var updatedEntity = await _repository.Update(entity);
                var result = _mapper.Map<TDto>(updatedEntity);
                return OperationResult<TDto>.Succeeded(_localizer["EntityUpdatedSuccessfully"], result);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TDto>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TEntity>> Delete(int id, bool physicalDelete = false)
        {
            try
            {
                //Check if the entity has any child records
                bool hasChildRecords = _repository.HasChildRecords<TEntity>(id);

                //If there are child records and not performing physical delete, return appropriate message
                if (hasChildRecords && !physicalDelete)
                {
                    return OperationResult<TEntity>.Failed(_localizer["CannotSoftDeleteWithChildRecords"]);
                }
                var entity = await _repository.Remove(id, physicalDelete);
                return OperationResult<TEntity>.Succeeded(_localizer["EntityDeletedSuccessfully"], entity);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<TEntity>> Delete(TEntity entity, bool physicalDelete = false)
        {
            try
            {
                //Check if the entity has any child records
                bool hasChildRecords = _repository.HasChildRecords<TEntity>(entity.Id);

                //If there are child records and not performing physical delete, return appropriate message
                if (hasChildRecords && !physicalDelete)
                {
                    return OperationResult<TEntity>.Failed(_localizer["CannotSoftDeleteWithChildRecords"]);
                }
                await _repository.Remove(entity, physicalDelete);
                return OperationResult<TEntity>.Succeeded(_localizer["EntityDeletedSuccessfully"], entity);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<TEntity>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<IEnumerable<TEntity>>> Delete(IEnumerable<TEntity> entities, bool physicalDelete = false)
        {
            try
            {
                await _repository.Remove(entities, physicalDelete);
                return OperationResult<IEnumerable<TEntity>>.Succeeded(_localizer["EntitiesDeletedSuccessfully"], entities);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual async Task<OperationResult<IEnumerable<TEntity>>> Delete(IEnumerable<int> ids)
        {
            try
            {
                var entities = await _repository.Remove(ids);
                return OperationResult<IEnumerable<TEntity>>.Succeeded(_localizer["EntitiesDeletedSuccessfully"], entities);
            }
            catch (RepositoryException ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<TEntity>>.InternalServerError(_localizer["UnexpectedError"]);
            }
        }

        public virtual TEntity MapToEntity(TDto entityDto) => _mapper.Map<TEntity>(entityDto);
        public virtual IEnumerable<TEntity> Map(IEnumerable<TDto> entityDtos) => _mapper.Map<IEnumerable<TEntity>>(entityDtos);
        public virtual TDto MapToDto(TEntity entity) => _mapper.Map<TDto>(entity);
        public virtual IEnumerable<TDto> Map(IEnumerable<TEntity> entities) => _mapper.Map<IEnumerable<TDto>>(entities);

        public virtual void SetProperty<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression, TProperty value)
        {
            if (!(propertyExpression.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyExpression}' refers to a method, not a property.");

            var propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException($"Expression '{propertyExpression}' refers to a field, not a property.");

            entity.GetType().GetProperty(propertyInfo.Name)?.SetValue(entity, value, null);
        }
    }
}

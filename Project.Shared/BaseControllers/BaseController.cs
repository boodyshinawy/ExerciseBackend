using Microsoft.AspNetCore.Mvc;


using System.Threading.Tasks;

namespace Project.Shared
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<TEntity, TDto> : ControllerBase
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        private readonly IBaseService<TEntity, TDto> _baseService;

        public BaseController(IBaseService<TEntity, TDto> service) => _baseService = service;

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            var entityResult = await _baseService.Read(id);
            return entityResult.Success
                ? this.AppSuccess(entityResult)
                : this.AppNotFound(entityResult.Message);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            var entitiesResult = await _baseService.Read();
            return entitiesResult.Success
                ? AppSuccess(entitiesResult)
                : this.AppNotFound(entitiesResult.Message);
        }

        //[HttpGet("Paging")]
        //public virtual async Task<IActionResult> Get([FromQuery] int page, [FromQuery] int pageSize)
        //{
        //    var entitiesResult = await _baseService.Read(page, pageSize);
        //    return entitiesResult.Success
        //        ? this.AppSuccess(entitiesResult)
        //        : this.AppNotFound(entitiesResult.Message);
        //}

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(int id, TDto entityDto)
        {
            var updatedEntityResult = await _baseService.Update(id, entityDto);
            return updatedEntityResult.Success
                ? this.AppSuccess(updatedEntityResult)
                : this.AppFailed(updatedEntityResult.Message);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(TDto dto)
        {
            var createdEntityResult = await _baseService.Create(dto);
            return createdEntityResult.Success
                ? this.AppSuccess(createdEntityResult)
                : this.AppFailed(createdEntityResult.Message);
        }

        //[HttpPost("AddBulk")]
        //public virtual async Task<IActionResult> Post(IEnumerable<TDto> entities)
        //{
        //    var createdEntitiesResult = await _baseService.Create(entities);
        //    return createdEntitiesResult.Success
        //        ? this.AppSuccess(createdEntitiesResult)
        //        : this.AppFailed(createdEntitiesResult.Message);
        //}

        //[HttpPut]
        //public virtual async Task<IActionResult> Put(TEntity entity)
        //{
        //    var updatedEntityResult = await _baseService.Update(entity);
        //    return updatedEntityResult.Success
        //        ? this.AppSuccess(updatedEntityResult)
        //        : this.AppFailed(updatedEntityResult.Message);
        //}

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var deletedEntityResult = await _baseService.Delete(id);
            return deletedEntityResult.Success
                ? this.AppSuccess(deletedEntityResult)
                : this.AppFailed(deletedEntityResult.Message);
        }

        //[HttpDelete]
        //public virtual async Task<IActionResult> Delete(TEntity entity)
        //{
        //    var deletedEntityResult = await _baseService.Delete(entity);
        //    return deletedEntityResult.Success
        //        ? this.AppSuccess(deletedEntityResult)
        //        : this.AppFailed(deletedEntityResult.Message);
        //}

        //[HttpDelete("DeleteBulk")]
        //public virtual async Task<IActionResult> Delete(IEnumerable<TEntity> entities)
        //{
        //    var deletedEntitiesResult = await _baseService.Delete(entities);
        //    return deletedEntitiesResult.Success
        //        ? this.AppSuccess(deletedEntitiesResult)
        //        : this.AppFailed(deletedEntitiesResult.Message);
        //}

        //[HttpDelete("DeleteByIds")]
        //public virtual async Task<IActionResult> DeleteByIds([FromBody] IEnumerable<int> ids)
        //{
        //    var deletedEntitiesResult = await _baseService.Delete(ids);
        //    return deletedEntitiesResult.Success
        //        ? this.AppSuccess(deletedEntitiesResult)
        //        : this.AppFailed(deletedEntitiesResult.Message);
        //}

        //[HttpDelete("ActualDelete/{id}")]
        //public virtual async Task<IActionResult> ActualDelete(int id)
        //{
        //    var deletedEntityResult = await _baseService.Delete(id, true);
        //    return deletedEntityResult.Success
        //        ? this.AppSuccess(deletedEntityResult)
        //        : this.AppFailed(deletedEntityResult.Message);
        //}

        protected IActionResult AppSuccess<T>(OperationResult<T> result)
        {
            return Ok(new { success = true, message = result.Message, data = result.Data });
        }

        protected IActionResult AppFailed(string message)
        {
            return BadRequest(new { success = false, message });
        }

        protected IActionResult AppNotFound(string message)
        {
            return NotFound(new { success = false, message});
        }

        protected IActionResult AppInternalServerError(string message)
        {
            return StatusCode(500, new { success = false, message });
        }
    }
}

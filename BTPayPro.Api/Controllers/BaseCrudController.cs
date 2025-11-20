using BTPayPro.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BTPayPro.Api.Controllers
{
    [ApiController]
    public abstract class BaseCrudController<TEntity, TRequestDto, TResponseDto, TService> : ControllerBase
        where TEntity : class
        where TRequestDto : class
        where TResponseDto : class
    {
        protected readonly TService _service;
        protected readonly MethodInfo _getAllMethod;
        protected readonly MethodInfo _getByIdMethod;
        protected readonly MethodInfo _addMethod;
        protected readonly MethodInfo _updateMethod;
        protected readonly MethodInfo _deleteMethod;

        public BaseCrudController(TService service)
        {
            _service = service;
            var serviceType = typeof(TService);

            // Use reflection to find the generic service methods
            _getAllMethod = serviceType.GetMethod("GetAllAsync") ?? throw new InvalidOperationException("Service must implement GetAllAsync.");
            _getByIdMethod = serviceType.GetMethod("GetByIdAsync") ?? throw new InvalidOperationException("Service must implement GetByIdAsync.");
            _addMethod = serviceType.GetMethod("AddAsync") ?? throw new InvalidOperationException("Service must implement AddAsync.");
            _updateMethod = serviceType.GetMethod("UpdateAsync") ?? throw new InvalidOperationException("Service must implement UpdateAsync.");
            _deleteMethod = serviceType.GetMethod("DeleteAsync") ?? throw new InvalidOperationException("Service must implement DeleteAsync.");
        }

        // GET: api/[controller]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TResponseDto>>> GetAll()
        {
            var entities = await (Task<IEnumerable<TEntity>>)_getAllMethod.Invoke(_service, null)!;
            var dtos = entities.Select(e => (TResponseDto)typeof(DtoMapper).GetMethod("ToDto", new[] { typeof(TEntity) })!.Invoke(null, new object[] { e })!).ToList();
            return Ok(dtos);
        }

        // GET: api/[controller]/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TResponseDto>> GetById(string id)
        {
            var entity = await (Task<TEntity?>)_getByIdMethod.Invoke(_service, new object[] { id })!;
            if (entity == null)
            {
                return NotFound();
            }
            var dto = (TResponseDto)typeof(DtoMapper).GetMethod("ToDto", new[] { typeof(TEntity) })!.Invoke(null, new object[] { entity })!;
            return Ok(dto);
        }

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult<TResponseDto>> Add([FromBody] TRequestDto dto)
        {
            var entity = (TEntity)typeof(DtoMapper).GetMethod("ToEntity", new[] { typeof(TRequestDto), typeof(string) })!.Invoke(null, new object[] { dto, null! })!;
            await (Task)_addMethod.Invoke(_service, new object[] { entity })!;

            // Assuming the entity now has its ID populated after AddAsync
            var responseDto = (TResponseDto)typeof(DtoMapper).GetMethod("ToDto", new[] { typeof(TEntity) })!.Invoke(null, new object[] { entity })!;

            // Attempt to get the ID property to use in CreatedAtAction
            var idProperty = typeof(TEntity).GetProperty("Id") ?? typeof(TEntity).GetProperty("TransactionId") ?? typeof(TEntity).GetProperty("WalletId") ?? typeof(TEntity).GetProperty("AccountId") ?? typeof(TEntity).GetProperty("UserId");
            var idValue = idProperty?.GetValue(entity)?.ToString() ?? "unknown";

            return CreatedAtAction("GetById", new { id = idValue }, responseDto);
        }

        // PUT: api/[controller]/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] TRequestDto dto)
        {
            var existingEntity = await (Task<TEntity?>)_getByIdMethod.Invoke(_service, new object[] { id })!;
            if (existingEntity == null)
            {
                return NotFound();
            }

            // Map DTO to entity, preserving the existing ID
            var updatedEntity = (TEntity)typeof(DtoMapper).GetMethod("ToEntity", new[] { typeof(TRequestDto), typeof(string) })!.Invoke(null, new object[] { dto, id })!;

            // Copy properties from updatedEntity to existingEntity to ensure EF tracks changes correctly
            foreach (var prop in typeof(TEntity).GetProperties())
            {
                if (prop.CanWrite && prop.Name != "Id" && prop.Name != "TransactionId" && prop.Name != "WalletId" && prop.Name != "AccountId" && prop.Name != "UserId")
                {
                    prop.SetValue(existingEntity, prop.GetValue(updatedEntity));
                }
            }

            await (Task)_updateMethod.Invoke(_service, new object[] { existingEntity })!;
            return NoContent();
        }

        // DELETE: api/[controller]/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await (Task<TEntity?>)_getByIdMethod.Invoke(_service, new object[] { id })!;
            if (entity == null)
            {
                return NotFound();
            }

            await (Task)_deleteMethod.Invoke(_service, new object[] { id })!;
            return NoContent();
        }
    }
}
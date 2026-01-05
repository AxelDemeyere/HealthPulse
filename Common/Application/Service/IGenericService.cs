using Common.Domain.Repository;

namespace Common.Application.Service;

public interface IGenericService<TEntity, TReceiveDto> where TEntity : class
{
    Task<TEntity> CreateAsync(TReceiveDto dto);
    Task<TEntity> UpdateAsync(int id, TReceiveDto dto);
    Task<bool> DeleteAsync(int id);
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
}

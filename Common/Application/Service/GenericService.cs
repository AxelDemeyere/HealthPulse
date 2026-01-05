using Common.Domain.Repository;

namespace Common.Application.Service;

public abstract class GenericService<TEntity, TReceiveDto> : IGenericService<TEntity, TReceiveDto> 
    where TEntity : class
{
    protected readonly IGenericRepository<TEntity> Repository;

    protected GenericService(IGenericRepository<TEntity> repository)
    {
        Repository = repository;
    }

    public abstract Task<TEntity> CreateAsync(TReceiveDto dto);
    public abstract Task<TEntity> UpdateAsync(int id, TReceiveDto dto);

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await Repository.GetByIdAsync(id);
        if (entity == null) return false;

        await Repository.DeleteAsync(entity);
        return true;
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }
}

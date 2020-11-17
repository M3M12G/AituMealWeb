using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AituMealWeb.Core.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<IReadOnlyList<TEntity>> GetAllRecs();
        Task<TEntity> GetRecById(Guid id);
        Task CreateRec(TEntity entity);
        Task UpdateRec(Guid id, TEntity entity);
        Task DeleteRec(Guid id);
    }
}

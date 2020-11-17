using AituMealWeb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AituMealWeb.Core.Interfaces.Repositories
{
    public interface IMealRepository:IGenericRepository<Meal>
    {
        Task<IReadOnlyList<Meal>> GetAllRecsJoin();
        Task<Meal> GetMealById(Guid id);
        Task<IReadOnlyList<Meal>> GetAllRecsJoinByCategoryId(Guid id);
    }
}

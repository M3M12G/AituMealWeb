using AituMealWeb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AituMealWeb.Core.Interfaces.Repositories
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        Task CreateMenu(IEnumerable<Menu> menuList);
        //methods below are using joins (Menu - Meals relation)
        Task<IReadOnlyList<Menu>> GetAllMenuJoin(DateTime date, bool instock);
        Task<Menu> GetMenuById(Guid id);
        Task DeleteForDate(DateTime date);
        Task<bool> isMealAvailable(Guid mealId);
    }
}

using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AituMealWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AituMealWeb.Infrastructure.Repositories
{
    public class MenuRepository : GenericRepository<Menu>, IMenuRepository
    {
        private readonly DataContext _dbContext;

        public MenuRepository(DataContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateMenu(IEnumerable<Menu> menuList)
        {
            await _dbContext.AddRangeAsync(menuList);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteForDate(DateTime date)
        {
            var atDate = _dbContext.Menu.Where(mu => mu.MenuForDay == date);
            _dbContext.RemoveRange(atDate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Menu>> GetAllMenuJoin(DateTime date, bool instock)//shows only todays menu according to day of week
        {
            return await _dbContext.Menu.Include(mu => mu.Meal).ThenInclude(m => m.MealCategory).AsNoTracking()
                .Where(mu => mu.InStock == instock && mu.MenuForDay == date)
                .OrderBy(mu => mu.MenuForDay)
                .ThenBy(mu => mu.DayTime)
                .ToListAsync();
        }

        public async Task<Menu> GetMenuById(Guid id)//Get the record of exact menu
        {
            return await _dbContext.Menu.Include(mu => mu.Meal).AsNoTracking().FirstOrDefaultAsync(mu => mu.Id == id);
        }

        public async Task<bool> isMealAvailable(Guid mealId)
        {
            var meal = await _dbContext.Menu.FirstOrDefaultAsync(mu => mu.MealId == mealId);
            if (meal == null)
                return false;

            if (meal.MenuForDay == DateTime.Today)
                if (meal.InStock.Equals(true))
                    return true;
            return false;
        }
    }
}

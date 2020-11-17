using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AituMealWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AituMealWeb.Infrastructure.Repositories
{
    public class MealRepository : GenericRepository<Meal>, IMealRepository
    {
        private readonly DataContext _dbContext;
        public MealRepository(DataContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        //methods below is used for retrieving data using joins for tables Meal and MealCategory
        public async Task<IReadOnlyList<Meal>> GetAllRecsJoin()
        {
            return await _dbContext.Meals.Include(x => x.MealCategory).AsNoTracking().OrderBy(x => x.Price).ToListAsync();
        }

        public async Task<IReadOnlyList<Meal>> GetAllRecsJoinByCategoryId(Guid id)
        {
            return await _dbContext.Meals.AsNoTracking().Include(x => x.MealCategory).Where(m => m.MealCategoryId == id).ToListAsync();
        }

        public async Task<Meal> GetMealById(Guid id)
        {
            return await _dbContext.Meals.Include(x => x.MealCategory).AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}

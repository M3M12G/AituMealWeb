using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AituMealWeb.Infrastructure.Data;

namespace AituMealWeb.Infrastructure.Repositories
{
    public class MealCategoryRepository : GenericRepository<MealCategory>, IMealCategoryRepository
    {
        public MealCategoryRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}

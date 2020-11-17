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
    public class OrderRepository : GenericRepository<OrderDetails>, IOrderRepository
    {
        private readonly DataContext _dbContext;

        public OrderRepository(DataContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task CreateOrder(IEnumerable<OrderDetails> orderBucket)
        {
            await _dbContext.AddRangeAsync(orderBucket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDetails>> GetAllOrderReceipt(DateTime ordDate, string Status)
        {
            return await _dbContext.OrderDetails.Include(o => o.User)
                                                .Include(o => o.Meals)
                                                .AsNoTracking()
                                                .OrderBy(o => o.Id)
                                                .ThenBy(o => o.UserId)
                                                .OrderBy(o => o.MealId)
                                                .OrderByDescending(o => o.OrderDate)
                                                .Where(o => o.OrderDate <= ordDate && o.Status == Status)
                                                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetails>> GetAllMyOrders(Guid userId)
        {
            return await _dbContext.OrderDetails.Include(o => o.User)
                                                .Include(o => o.Meals)
                                                .AsNoTracking()
                                                .OrderBy(o => o.Id)
                                                .ThenBy(o => o.UserId)
                                                .OrderBy(o => o.MealId)
                                                .OrderByDescending(o => o.OrderDate)
                                                .Where(o => o.UserId == userId)
                                                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetails>> GetAllOrderReceiptToday()//kassir see in process orders
        {
            return await _dbContext.OrderDetails.Include(o => o.User)
                                                .Include(o => o.Meals)
                                                .AsNoTracking()
                                                .OrderBy(o => o.Id)
                                                .ThenBy(o => o.UserId)
                                                .OrderBy(o => o.MealId)
                                                .OrderBy(o => o.OrderDate)
                                                .Where(o => o.OrderDate >= DateTime.Now.Date && o.Status == Status.Process)
                                                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetails>> GetOrderById(Guid orderId)
        {
            return await _dbContext.OrderDetails.Include(o => o.User)
                                                .Include(o => o.Meals)
                                                .AsNoTracking()
                                                .Where(o => o.Id == orderId)
                                                .ToListAsync();
                                                
        }

        public async Task UpdateOrderStatus(Guid orderId, string status)
        {
            await _dbContext.OrderDetails.Where(ord => ord.Id == orderId).ForEachAsync(ord => ord.Status = status);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteCancelledOrders()
        {
            var cancOrd = await _dbContext.OrderDetails.Where(ord => ord.Status == Status.Cancel).ToListAsync();
            if (cancOrd.Count == 0)
                return false; 

            _dbContext.OrderDetails.RemoveRange(cancOrd);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}

using AituMealWeb.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AituMealWeb.Core.Interfaces.Repositories
{
    public interface IOrderRepository:IGenericRepository<OrderDetails>
    {
        Task CreateOrder(IEnumerable<OrderDetails> orderBucket);
        Task<IEnumerable<OrderDetails>> GetAllOrderReceipt(DateTime ordDate, string Status);//admin by default can see completed orders
                                                                                            //also available observing in process or cancelled orders
                                                                                            //if it is defined by Status parameter
                                                                                            //also can be used to retrieve orders report
        Task<IEnumerable<OrderDetails>> GetAllMyOrders(Guid user);//retrieves orders of exact user
        Task<IEnumerable<OrderDetails>> GetAllOrderReceiptToday();//kassirs retrieve incoming orders
        Task<IEnumerable<OrderDetails>> GetOrderById(Guid orderId);
        Task UpdateOrderStatus(Guid orderId, string status);//used to cancel or complete order
        Task<bool> DeleteCancelledOrders();
    }
}

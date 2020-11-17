using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO.MealDTOs;
using AituMealWeb.Core.DTO.OrderDetailDTOs;
using AituMealWeb.Core.DTO.UserDTOs;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AituMealWeb.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;

        public OrdersController(IOrderRepository orderRepository, IMenuRepository menuRepository)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
        }

        [HttpPost("new")]//everyone can make order
        public async Task<IActionResult> MakeMealOrder(List<CreateOrder> userBucket)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            { 
                var OrderId = Guid.NewGuid();
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userIdClaim == null)
                    return BadRequest(NotFound("May be you should pass authentication!"));
                
                foreach(Guid mealId in userBucket.Select(m => m.MealId))
                {
                    var check = await _menuRepository.isMealAvailable(mealId);
                    if(!check)
                        return BadRequest("Sorry, one or more of meals on your Meal bucket is now not available for purchase! Please, choose another or delete those!");
                }

                List<OrderDetails> toSaveOrder = new List<OrderDetails>();

                foreach (CreateOrder o in userBucket) {
                    OrderDetails singleOrder = new OrderDetails()
                    {
                        Id = OrderId,
                        UserId = Guid.Parse(userIdClaim.ToString()),
                        MealId = o.MealId,
                        Quantity = (ushort)o.Quantity,
                        AmountBought = (decimal)o.AmountBought,
                        OrderDate = DateTime.Now,
                        Status = Status.Process
                    };
                    toSaveOrder.Add(singleOrder);
                }

                await _orderRepository.CreateOrder(toSaveOrder);
                var orderPageURL = $"/api/orders/myorders/{OrderId}";//redirecting user to its orders
                return Ok(Redirect(orderPageURL));
            }
            catch
            {
                return BadRequest("Some problems occured during Order creation process!");
            }
        }

        [Authorize(Roles = Role.Admin)]//админ волен выбирать дату заказов и фильтровать по статусу заказа
        [HttpGet("{orddate:DateTime?}/{status?}")]//by default админ увидит выполненные заказы
        public async Task<IActionResult> GetAllOrders(DateTime? orddate = null, string status = Status.Complete)
        {

            if (orddate == null)//по умолчанию, показываются записи до сегоднящнего дня.
                orddate = DateTime.Today.AddHours(23);
            else
                orddate = orddate.Value.AddHours(23);//так как клиент отправляет только дату, без уточнения времени, 
            //то чтобы увидеть заказы до того дня включительно, надо указать конец того дня, потому что время по дефолту T00:00:00

            var orders = await _orderRepository.GetAllOrderReceipt((DateTime)orddate, status);//все записи с бд до указанной даты
            ICollection<OrdersDTO> OrdersList = new LinkedList<OrdersDTO>();

            var orderIds = orders.Select(ord => ord.Id).Distinct();//Берем id заказов и итерируемся по нему по записям заказов

            foreach (Guid orderId in orderIds)
            {
                ICollection<MealToMap> mealCart = new LinkedList<MealToMap>();//так как в заказе может быть от 1 до нескольких
                //единиц еды, они помещяються в корзинку еды соответственно id заказа
                
                foreach(var oin in orders.Where(ord => ord.Id == orderId))
                {
                    mealCart.Add(
                        new MealToMap()
                        {
                            MealId = oin.MealId,
                            MealName = oin.Meals.Name,
                            MealAmount = (decimal)oin.Meals.Amount,
                            MealPrice = (decimal)oin.Meals.Price * oin.AmountBought * oin.Quantity,
                            OrderDetailsAmountBought = oin.AmountBought,
                            OrderDetailsQuantity = oin.Quantity
                        }
                        );
                    
                }

                var total = mealCart.Select(m => m.MealPrice).Sum();// используя корзинку еды, получаем сумму заказа

                OrderDetails sOrd = orders.FirstOrDefault(ord => ord.Id == orderId);//получаем конкретный заказ
                //а именно, такие поля как Id, User, OrderDate, Status. 

                OrdersDTO singleOrder = new OrdersDTO()
                {
                    Id = orderId,
                    User = new UserToMap()
                    {
                        Id = sOrd.UserId,
                        FirstName = sOrd.User.FirstName,
                        LastName = sOrd.User.LastName
                    },
                    MealBucket = mealCart,
                    OrderDate = sOrd.OrderDate,
                    TotalPrice = total,
                    Status = sOrd.Status
                    
                };
                OrdersList.Add(singleOrder);
            }
            return orders != null ? (IActionResult)Ok(OrdersList) : NoContent();
        }


        [Authorize(Roles = Role.Kassir)]//Вывод данных заказов для кассиров за сегодня начиная с 00:00
        [HttpGet("incoming")]
        public async Task<IActionResult> GetAllOrdersKassa()
        {
            var orders = await _orderRepository.GetAllOrderReceiptToday();//все записи с бд
            ICollection<OrdersDTO> OrdersList = new LinkedList<OrdersDTO>();

            var orderIds = orders.Select(ord => ord.Id).Distinct();//Берем id заказов и итерируемся по нему по записям заказов

            foreach (Guid orderId in orderIds)
            {
                ICollection<MealToMap> mealCart = new LinkedList<MealToMap>();//так как в заказе может быть от 1 до нескольких
                                                                              //единиц еды, они помещяються в корзинку еды соответственно id заказа

                foreach (var oin in orders.Where(ord => ord.Id == orderId))
                {
                    mealCart
                        .Add(
                        new MealToMap()
                        {   MealId = oin.MealId,
                            MealName = oin.Meals.Name,
                            MealAmount = (decimal)oin.Meals.Amount,
                            MealPrice = (decimal)oin.Meals.Price * oin.AmountBought * oin.Quantity,
                            OrderDetailsAmountBought = oin.AmountBought,
                            OrderDetailsQuantity = oin.Quantity
                        }   ); 

                }

                var total = mealCart.Select(m => m.MealPrice).Sum();// используя корзинку еды, получаем сумму заказа

                OrderDetails sOrd = orders.FirstOrDefault(ord => ord.Id == orderId);//получаем конкретный заказ
                //а именно, такие поля как Id, User, OrderDate, Status. 

                OrdersDTO singleOrder = new OrdersDTO()
                {
                    Id = orderId,
                    User = new UserToMap()
                    {
                        Id = sOrd.UserId,
                        FirstName = sOrd.User.FirstName,
                        LastName = sOrd.User.LastName
                    },
                    MealBucket = mealCart,
                    OrderDate = sOrd.OrderDate,
                    TotalPrice = (decimal)total,
                    Status = sOrd.Status

                };
                OrdersList.Add(singleOrder);
            }
            return orders != null ? (IActionResult)Ok(OrdersList) : NoContent();
        }
        
        [Authorize(Roles = Role.AdminOrKassir)]//Get exact order by orderId
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetailsById(Guid id)
        {
            if (id == null)
                return NotFound("Order not found!");

            var orderById = await _orderRepository.GetOrderById(id);

            ICollection<MealToMap> mealCart = new LinkedList<MealToMap>();//так как в заказе может быть от 1 до нескольких
                                                                          //единиц еды, они помещяються в корзинку еды соответственно id заказа
            foreach (OrderDetails o in orderById)
            {
                mealCart.Add(
                        new MealToMap()
                        {
                            MealId = o.MealId,
                            MealName = o.Meals.Name,
                            MealAmount = (decimal)o.Meals.Amount,
                            MealPrice = (decimal)o.Meals.Price * o.AmountBought * o.Quantity,
                            OrderDetailsAmountBought = o.AmountBought,
                            OrderDetailsQuantity = o.Quantity
                        }
                        );
            }

            OrderDetails sOrd = orderById.FirstOrDefault(ord => ord.Id == id);//получаем конкретный заказ
                                                                              //а именно, такие поля как Id, User, OrderDate, Status. 
            var total = mealCart.Select(m => m.MealPrice).Sum();// используя корзинку еды, получаем сумму заказа

            OrdersDTO singleOrder = new OrdersDTO()
            {
                Id = sOrd.Id,
                User = new UserToMap()
                {
                    Id = sOrd.UserId,
                    FirstName = sOrd.User.FirstName,
                    LastName = sOrd.User.LastName
                },
                MealBucket = mealCart,
                OrderDate = sOrd.OrderDate,
                TotalPrice = (decimal)total,
                Status = sOrd.Status

            };

            return orderById != null ? (IActionResult)Ok(singleOrder) : NotFound($"No records found for this Order {id}");
        }

        [HttpGet("myorders/{id}")]//for users get orders of exact user
        public async Task<IActionResult> GetUserOrder(Guid id)
        {
            if (id == null)
                return NotFound("Oops, your order not found!");

            return await GetOrderDetailsById(id);
        }

        [HttpGet("myorders")]//this is basically for users, but others also can use it
                            //used to see the list of all orders for all time
        public async Task<IActionResult> GetUserOrders()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userIdClaim == null)
                return BadRequest(NotFound("Could not find user id"));

            var myOrders = await _orderRepository.GetAllMyOrders(Guid.Parse(userIdClaim.ToString()));

            ICollection<OrdersDTO> OrdersList = new LinkedList<OrdersDTO>();

            var orderIds = myOrders.Select(ord => ord.Id).Distinct();//Берем id заказов и итерируемся по нему по записям заказов

            foreach (Guid orderId in orderIds)
            {
                ICollection<MealToMap> mealCart = new LinkedList<MealToMap>();//так как в заказе может быть от 1 до нескольких
                                                                              //единиц еды, они помещяються в корзинку еды соответственно id заказа

                foreach (var oin in myOrders.Where(ord => ord.Id == orderId))
                {
                    mealCart.Add(
                        new MealToMap()
                        {
                            MealId = oin.MealId,
                            MealName = oin.Meals.Name,
                            MealAmount = (decimal)oin.Meals.Amount,
                            MealPrice = (decimal)oin.Meals.Price * oin.AmountBought * oin.Quantity,
                            OrderDetailsAmountBought = oin.AmountBought,
                            OrderDetailsQuantity = oin.Quantity
                        }
                        );

                }

                var total = mealCart.Select(m => m.MealPrice).Sum();// используя корзинку еды, получаем сумму заказа

                OrderDetails sOrd = myOrders.FirstOrDefault(ord => ord.Id == orderId);//получаем конкретный заказ
                //а именно, такие поля как Id, User, OrderDate, Status. 

                OrdersDTO singleOrder = new OrdersDTO()
                {
                    Id = orderId,
                    User = new UserToMap()
                    {
                        Id = sOrd.UserId,
                        FirstName = sOrd.User.FirstName,
                        LastName = sOrd.User.LastName
                    },
                    MealBucket = mealCart,
                    OrderDate = sOrd.OrderDate,
                    TotalPrice = (decimal)total,
                    Status = sOrd.Status

                };
                OrdersList.Add(singleOrder);
            }
            return myOrders != null ? (IActionResult)Ok(OrdersList) : NoContent();
        }

        [HttpPut("change/{status:int}/{id}")]//Used to Cancel/Complete order
        public async Task<IActionResult> ChangeOrderStatus(int status, Guid id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return BadRequest(NotFound("No order records found!"));

            string statusToSet;

            switch (status)
            {
                case 1:
                    statusToSet = Status.Complete;
                    break;
                case 2:
                    statusToSet = Status.Cancel;
                    break;
                default:
                    statusToSet = Status.Process;
                    break;
            }

            try
            {
                await _orderRepository.UpdateOrderStatus(id, statusToSet);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occured during Order process!");
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("deletion/cancelled")]
        public async Task<IActionResult> DeleteAllCancelledOrders()
        {
            try
            {
                bool isDeleted = await _orderRepository.DeleteCancelledOrders();

                if (isDeleted)
                    return Ok();
                else
                    return BadRequest("No orders are cancelled to remove");
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("report/{orddate:DateTime?}")]
        public async Task<IActionResult> GetReportOfTodaysOrders(DateTime? orddate = null)
        {
            if (orddate == null)//по умолчанию, показываются записи до сегоднящнего дня.
                orddate = DateTime.Today.AddHours(23);
            else
                orddate = orddate.Value.AddHours(23);//так как клиент отправляет только дату, без уточнения времени, 
            //то чтобы увидеть заказы до того дня включительно, надо указать конец того дня, потому что время по дефолту T00:00:00

            var allOrders = await _orderRepository.GetAllOrderReceipt((DateTime)orddate, Status.Complete);

            var numb = allOrders.ToList().Count;

            if (numb == 0)
                return BadRequest($"No orders completed for date {orddate}");

            var ordersCompleted = allOrders.Select(ord => ord.Id).Distinct().Count();
            var totalOrdersPrice = allOrders.Select(ord => ord.Meals.Price * ord.Quantity * ord.AmountBought).Sum();

            var mealIds = allOrders.Select(ord => ord.MealId).Distinct();
            ICollection<MealStat> exactMeals = new LinkedList<MealStat>();

            foreach(Guid mealN in mealIds)
            {
                var mealSoldQuantities = allOrders.Where(ord => ord.MealId == mealN).Select(m => (int)m.Quantity).Sum();
                var mealInfo = allOrders.FirstOrDefault(ord => ord.MealId == mealN);

                MealStat mealRecord = new MealStat()
                {
                    MealId = mealN,
                    MealName = mealInfo.Meals.Name,
                    MealAmount = (decimal)mealInfo.Meals.Amount,
                    MealPrice = (decimal)mealInfo.Meals.Price,
                    SoldQuantity = mealSoldQuantities
                };
                exactMeals.Add(mealRecord);
            }

            OrdersReport report = new OrdersReport()
            {
                OrderDate = (DateTime)orddate,
                OrdersMade = ordersCompleted,
                FoodSale = exactMeals,
                TotalPrice = (decimal)totalOrdersPrice
            };

            return Ok(report);
        }
    }
}


using AituMealWeb.Core.DTO.MealDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.OrderDetailDTOs
{
    public class OrdersReport
    {
        public DateTime OrderDate { get; set; }
        public int OrdersMade { get; set; }
        public ICollection<MealStat> FoodSale { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

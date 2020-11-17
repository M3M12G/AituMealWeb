using AituMealWeb.Core.DTO.MealDTOs;
using AituMealWeb.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.OrderDetailDTOs
{
    public class OrdersDTO
    {
        public Guid Id { get; set; }
        public UserToMap User { get; set; }
        public ICollection<MealToMap> MealBucket { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}

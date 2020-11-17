using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.MealDTOs
{
    public class MealToMap
    {
        public Guid MealId { get; set; }
        public string MealName { get; set; }
        public decimal MealAmount { get; set; }
        public decimal MealPrice { get; set; }
        public ushort OrderDetailsQuantity { get; set; }
        public decimal OrderDetailsAmountBought { get; set; }
    }
}

using System;

namespace AituMealWeb.Core.DTO.MealDTOs
{
    public class MealStat
    {
        public Guid MealId { get; set; }
        public string MealName { get; set; }
        public decimal MealAmount { get; set; }
        public decimal MealPrice { get; set; }
        public int SoldQuantity { get; set; }
    }
}

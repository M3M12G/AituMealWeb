using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.MenuDTOs
{
    public class MenuDetails
    {
        public Guid Id { get; set; }
        public DateTime MenuForDay { get; set; }
        public int DayTime { get; set; }// for example, breakfast or lunch
        public Guid MealId { get; set; } // exact meal id and below the meal details
        public string MealName { get; set; }
        public string MealPicture { get; set; }
        public decimal MealAmount { get; set; }
        public decimal MealPrice { get; set; }
        public Guid MealMealCategoryId { get; set; }
        public string MealMealCategoryCategory { get; set; }
        public bool InStock { get; set; }//in case if in canteen some meal would be out of stock
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.Entities
{
    public class Menu : IEntity
    {
        public Guid Id { get; set; }
        public DateTime MenuForDay { get; set; }
        public int DayTime { get; set; }//the order of day time: 1 - breakfast, 2 - lunch, 3 - dinner
        public Guid MealId { get; set; } // exact meal id
        public Meal Meal { get; set; }
        public bool InStock { get; set; }//in case if in canteen some meal would be out of stock
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.MenuDTOs
{
    public class MenuScheduled
    {
        public DateTime MenuFor { get; set; }
        public ICollection<MealsByTime> MealsByTime { get; set; }
    }
}

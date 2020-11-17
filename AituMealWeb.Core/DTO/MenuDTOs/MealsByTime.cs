using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.DTO.MenuDTOs
{
    public class MealsByTime
    {
        public int DayTime;
        public ICollection<MealOnMenu> MealOnMenu { get; set; }
    }
}

using AituMealWeb.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AituMealWeb.Core.DTO.MenuDTOs
{
    public class CreateMenu
    {
        [Required(ErrorMessage = "Date of the Menu should be choosed!")]
        public DateTime MenuForDay { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "Time of the day should be choosed!")]
        public int DayTime { get; set; }// for example, 1- breakfast or 2-lunch
        [Required(ErrorMessage = "Meal id is required")]
        public Guid MealId { get; set; } // exact meal id
        public bool InStock { get; set; } = true;//in case if in canteen some meal would be out of stock

    }
}

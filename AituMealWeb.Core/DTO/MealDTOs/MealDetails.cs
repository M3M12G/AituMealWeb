using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AituMealWeb.Core.DTO
{
    public class MealDetails
    {
        ////USED FOR MAPPING DETAILS FROM DB WITH CLIENT-ORIENTED DATA
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Meal name should not be empty")]
        public string Name { get; set; }
        [DefaultValue("https://icon-library.com/images/meal-icon-png/meal-icon-png-26.jpg")] //default picture of meal
        public string Picture { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Meal category is required")]
        public Guid MealCategoryId { get; set; }
        public string MealCategoryCategory { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AituMealWeb.Core.DTO
{
    public class MealCreate
    {
        //USED FOR CREATING NEW MEAL
        [Required(ErrorMessage = "Meal name is required")]
        [StringLength(30, ErrorMessage = "Meal name length cannot be longer than 20 symbols")]
        public string Name { get; set; }
        public string Picture { get; set; } = "https://icon-library.com/images/meal-icon-png/meal-icon-png-26.jpg";
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Meal category is required")]
        public Guid MealCategoryId { get; set; }
    }
}

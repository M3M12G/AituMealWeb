using System;
using System.ComponentModel.DataAnnotations;

namespace AituMealWeb.Core.DTO
{
    public class MealCategoryDetails
    {
        //USED FOR MAPPING DETAILS FROM DB WITH CLIENT-ORIENTED DATA
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(20, ErrorMessage = "Category name length cannot be longer than 20 symbols")]
        public string Category { get; set; }
    }
}

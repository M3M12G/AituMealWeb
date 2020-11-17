using System;
using System.Collections;
using System.Collections.Generic;

namespace AituMealWeb.Core.Entities
{
    public class Meal:IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public Guid MealCategoryId { get; set; }
        public MealCategory MealCategory { get; set; }

        public ICollection<Menu> Menus { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }

    }
}

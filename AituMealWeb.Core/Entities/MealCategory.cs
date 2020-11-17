using System;
using System.Collections.Generic;

namespace AituMealWeb.Core.Entities
{
    public class MealCategory:IEntity
    {
        public Guid Id { get; set; } //unique ID
        public string Category { get; set; }
        public ICollection<Meal> Meals { get; set; }
    }
}
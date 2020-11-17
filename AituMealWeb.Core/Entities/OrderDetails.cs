using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AituMealWeb.Core.Entities
{
    public class OrderDetails : IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Key]
        public Guid MealId { get; set; }
        public ushort Quantity { get; set; }
        public decimal AmountBought { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public Meal Meals { get; set; }
        public User User { get; set; }
    }
}

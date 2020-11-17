using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AituMealWeb.Core.DTO.OrderDetailDTOs
{
    public class CreateOrder
    {
        //[Required(ErrorMessage = "User ID absent!")]
        //public Guid UserId { get; set; }
        [Required(ErrorMessage = "Meal ID should not be empty!")]
        public Guid MealId { get; set; }
        public ushort? Quantity { get; set; }
        public decimal? AmountBought { get; set; }
        //public DateTime OrderDate { get; set; } = DateTime.Now;
        //public string Status { get; set; } = "processing";
    }
}

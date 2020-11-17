using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AituMealWeb.Core.DTO.UserDTOs
{
    public class RegisterModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email length cannot be longer than 50 symbols")]
        public string Email { get; set; }
        [Required(ErrorMessage = "First name is required")]
        [StringLength(60, ErrorMessage = "First name length cannot be longer than 60 symbols")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(60, ErrorMessage = "Last name length cannot be longer than 60 symbols")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

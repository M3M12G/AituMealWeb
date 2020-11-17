using System;
using System.Collections.Generic;

namespace AituMealWeb.Core.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}

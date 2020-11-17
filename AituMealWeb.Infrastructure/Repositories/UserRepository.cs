using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AituMealWeb.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}

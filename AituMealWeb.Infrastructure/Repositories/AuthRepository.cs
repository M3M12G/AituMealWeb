using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AituMealWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AituMealWeb.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dbContext;
      
        public AuthRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
            
        }

        public async Task<User> Login(string email, string password)
        {
            var userByEmail = await _dbContext.UserList.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (userByEmail == null)
                return null;
            
            bool passVerification = VerifyPasswordHash(password, userByEmail.PasswordHash, userByEmail.PasswordSalt);
            if (!passVerification)
            {
                return null;
            }
            else {
                return userByEmail;
            }

        }

        public async Task<Guid> Register(User userDetails, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            userDetails.PasswordHash = passwordHash;
            userDetails.PasswordSalt = passwordSalt;

            await _dbContext.UserList.AddAsync(userDetails);
            await _dbContext.SaveChangesAsync();
            return userDetails.Id;
        }

        public bool UserExists(string email)
        {
            if (_dbContext.UserList.Any(u => u.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}

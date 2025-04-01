using DotCart.DAL.Interfaces;
using DotCart.Data;
using DotCart.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DotCart.DAL.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context) => _context = context;


        //this method will get the user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        //this method will check if the email exists in the database
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email!.ToLower() == email.ToLower());
        }
    }
}

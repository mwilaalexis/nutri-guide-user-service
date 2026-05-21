using Microsoft.EntityFrameworkCore;
using UserProject.DataAccess.Data;
using UserProject.DataAccess.Entities;
using UserProject.DataAccess.Repositories.Interfaces;

namespace UserProject.DataAccess.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(x => x.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.Include(x=>x.Profile).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);

        }
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);

        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<IEnumerable<User>> GetAllUsers(int page, int pageSize)
        {
            var users = await _context.Users
                                 .Include(x=>x.Profile)
                                 .OrderBy(u => u.CreatedAt)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            return users;

        }
    }
}

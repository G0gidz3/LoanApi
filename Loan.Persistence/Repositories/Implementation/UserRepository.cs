using Loan.Application.Repositories.Abstraction;
using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.Persistence.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        public ApplicationDbContext dbContext { get; }

        public UserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            user.Password = PasswordHasher.Hash(user.Password);
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        public async Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            User user = await dbContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return user;
        }

        public async Task<List<User>> GetUserlistAsync(CancellationToken cancellationToken)
        {
            List<User> userList = await dbContext.Users.ToListAsync(cancellationToken);
            return userList;
        }

        public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken)
        {
            User user = await dbContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Username == username && x.Password == PasswordHasher.Hash(password), cancellationToken);
            return user;
        }

        public async Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task BlockUserByIdAsync(int id, int minutes, CancellationToken cancellationToken)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.IsBlocked = true;
            user.BlockedUntil = DateTime.UtcNow.AddMinutes(minutes);

            dbContext.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UnBlockUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            User user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.IsBlocked = false;
            user.BlockedUntil = null;

            dbContext.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

using Loan.Domain.Entities;

namespace Loan.Application.Repositories.Abstraction
{
    public interface IUserRepository
    {
        public Task<int> CreateUserAsync(User user, CancellationToken cancellationToken);
        public Task<User> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        public Task<User> GetUserByUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken);
        public Task<List<User>> GetUserlistAsync(CancellationToken cancellationToken);
        public Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken);
    }
}

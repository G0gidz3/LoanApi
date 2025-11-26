using Loan.Domain.Entities;

namespace Loan.Application.Repositories.Abstraction
{
    public interface IRoleRepository
    {
        public Task<Role> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken);
    }
}

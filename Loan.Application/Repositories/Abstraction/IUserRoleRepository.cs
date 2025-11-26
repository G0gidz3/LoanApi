using Loan.Domain.Entities;

namespace Loan.Application.Repositories.Abstraction
{
    public interface IUserRoleRepository
    {
        public Task CreateUserRolesAsync(List<UserRole> userRoles, CancellationToken cancellationToken);
    }
}

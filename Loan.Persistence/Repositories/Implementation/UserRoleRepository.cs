using Loan.Application.Repositories.Abstraction;
using Loan.Domain.Entities;

namespace Loan.Persistence.Repositories.Implementation
{
    public class UserRoleRepository : IUserRoleRepository
    {
        public ApplicationDbContext dbContext { get; }

        public UserRoleRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateUserRolesAsync(List<UserRole> userRoles, CancellationToken cancellationToken)
        {
            await dbContext.UserRoles.AddRangeAsync(userRoles, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

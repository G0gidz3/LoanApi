using Loan.Application.Repositories.Abstraction;
using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.Persistence.Repositories.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        public ApplicationDbContext dbContext { get; }

        public RoleRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Role> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken)
        {
            Role role = await dbContext.Roles
                .FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);
            return role;
        }
    }
}

using Loan.Application.Models.LoanModels;
using Loan.Application.Repositories.Abstraction;
using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.Persistence.Repositories.Implementation
{
    public class LoanRepository : ILoanRepository
    {
        public ApplicationDbContext dbContext { get; }

        public LoanRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Domain.Entities.Loan> GetLoanByIdAsync(int id, CancellationToken cancellationToken)
        {
            Domain.Entities.Loan loan = await dbContext.Loans
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return loan;
        }

        public async Task<List<Domain.Entities.Loan>> GetLoanListByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Loan> loanList = await dbContext.Loans
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            return loanList;
        }

        public async Task<int> CreateLoanAsync(Domain.Entities.Loan loan, CancellationToken cancellationToken)
        {
            await dbContext.Loans.AddAsync(loan, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return loan.Id;
        }

        public async Task UpdateLoanAsync(UpdateLoanRequest updatedLoan, CancellationToken cancellationToken)
        {
            Domain.Entities.Loan dbLoan = await dbContext.Loans.FirstOrDefaultAsync(x => x.Id == updatedLoan.Id, cancellationToken);

            if (dbLoan == null)
            {
                throw new Exception("Loan not found");
            }

            dbLoan.LoanType = updatedLoan.LoanType;
            dbLoan.Amount = updatedLoan.Amount;
            dbLoan.DurationInMonths = updatedLoan.DurationInMonths;

            dbContext.Loans.Update(dbLoan);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteLoanAsync(int id, CancellationToken cancellationToken)
        {
            Domain.Entities.Loan dbLoan = await dbContext.Loans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (dbLoan == null)
            {
                throw new Exception("Loan not found");
            }

            dbContext.Loans.Remove(dbLoan);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

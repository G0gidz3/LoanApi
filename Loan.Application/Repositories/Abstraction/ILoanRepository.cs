using Loan.Application.Models.LoanModels;

namespace Loan.Application.Repositories.Abstraction
{
    public interface ILoanRepository
    {
        public Task<Domain.Entities.Loan> GetLoanByIdAsync(int id, CancellationToken cancellationToken);
        public Task<List<Domain.Entities.Loan>> GetLoanListByUserIdAsync(int userId, CancellationToken cancellationToken);
        public Task<int> CreateLoanAsync(Domain.Entities.Loan loan, CancellationToken cancellationToken);
        public Task UpdateLoanAsync(UpdateLoanRequest updatedLoan, CancellationToken cancellationToken);
        public Task DeleteLoanAsync(int id, CancellationToken cancellationToken);
    }
}

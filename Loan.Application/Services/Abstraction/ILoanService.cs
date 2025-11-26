using Loan.Application.Models.LoanModels;

namespace Loan.Application.Services.Abstraction
{
    public interface ILoanService
    {
        public Task<int> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken);
        public Task<LoanDetailsResponse> GetLoanByIdAsync(int id, CancellationToken cancellationToken);
        public Task<List<LoanDetailsResponse>> GetLoansByUserIdAsync(int userId, CancellationToken cancellationToken);
        public Task UpdateLoanAsync(UpdateLoanRequest request, CancellationToken cancellationToken);
        public Task DeleteLoanByIdAsync(int id, CancellationToken cancellationToken);
    }
}

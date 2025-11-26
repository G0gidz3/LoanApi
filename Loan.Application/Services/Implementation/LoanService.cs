using Loan.Application.Models.LoanModels;
using Loan.Application.Repositories.Abstraction;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Enums;

namespace Loan.Application.Services.Implementation
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository loanRepository;
        private readonly IUserRepository userRepository;

        public LoanService(ILoanRepository loanRepository, IUserRepository userRepository)
        {
            this.loanRepository = loanRepository;
            this.userRepository = userRepository;
        }

        public async Task<int> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user.IsBlocked || user.BlockedUntil > DateTime.UtcNow)
                throw new Exception($"User by id [{request.UserId}] is blocked until {user.BlockedUntil}");
            else
                await userRepository.UnBlockUserByIdAsync(user.Id, cancellationToken);

            var loan = new Domain.Entities.Loan
            {
                UserId = request.UserId,
                Amount = request.Amount,
                Currency = request.Currency ?? "GEL",
                LoanType = request.LoanType,
                LoanStatus = LoanStatus.Pending,
                DurationInMonths = request.DurationInMonths,
                CreateDate = DateTime.UtcNow
            };

            int id = await loanRepository.CreateLoanAsync(loan, cancellationToken);

            return id;
        }

        public async Task<LoanDetailsResponse> GetLoanByIdAsync(int id, CancellationToken cancellationToken)
        {
            var loan = await loanRepository.GetLoanByIdAsync(id, cancellationToken);
            if (loan == null)
                throw new Exception($"Loan by id [{id}] not found");

            var loanDetails = new LoanDetailsResponse
            {
                LoanId = loan.Id,
                UserId = loan.UserId,
                Firstname = loan.User.FirstName,
                Lastname = loan.User.LastName,
                Amount = loan.Amount,
                LoanType = loan.LoanType,
                Currency = loan.Currency,
                DurationInMonths = loan.DurationInMonths,
                LoanStatus = loan.LoanStatus,
                CreateDate = loan.CreateDate,
                TotalToPay = CalculateLoanTotal(loan.Amount, loan.DurationInMonths)
            };

            return loanDetails;
        }

        public async Task<List<LoanDetailsResponse>> GetLoansByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var loans = await loanRepository.GetLoanListByUserIdAsync(userId, cancellationToken);
            List<LoanDetailsResponse> loanlistDetails = loans.Select(loan => new LoanDetailsResponse
            {
                LoanId = loan.Id,
                UserId = loan.UserId,
                Firstname = loan.User.FirstName,
                Lastname = loan.User.LastName,
                Amount = loan.Amount,
                LoanType = loan.LoanType,
                Currency = loan.Currency,
                DurationInMonths = loan.DurationInMonths,
                LoanStatus = loan.LoanStatus,
                CreateDate = loan.CreateDate,
                TotalToPay = CalculateLoanTotal(loan.Amount, loan.DurationInMonths)
            }).ToList();

            return loanlistDetails;
        }

        public async Task UpdateLoanAsync(UpdateLoanRequest request, CancellationToken cancellationToken)
        {
            var loan = await loanRepository.GetLoanByIdAsync(request.Id, cancellationToken);
            if (loan == null)
                throw new Exception($"Loan by id [{request.Id}] not found");
            if (loan.LoanStatus != LoanStatus.Pending)
                throw new Exception($"Only loans with Pending status can be updated. Loan Id: [{request.Id}] has status [{loan.LoanStatus}]");
            await loanRepository.UpdateLoanAsync(request, cancellationToken);
        }

        public async Task DeleteLoanByIdAsync(int id, CancellationToken cancellationToken)
        {
            await loanRepository.DeleteLoanAsync(id, cancellationToken);
        }

        private decimal CalculateLoanTotal(decimal amount, int months)
        {
            const decimal yearlyInterest = 0.18m;
            decimal monthlyInterest = yearlyInterest / 12;

            return amount + (amount * monthlyInterest * months);
        }
    }
}

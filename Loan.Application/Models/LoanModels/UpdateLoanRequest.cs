using Loan.Domain.Enums;

namespace Loan.Application.Models.LoanModels
{
    public class UpdateLoanRequest
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public LoanType LoanType { get; set; }
        public int DurationInMonths { get; set; }
    }
}

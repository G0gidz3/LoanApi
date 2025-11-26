using Loan.Domain.Enums;

namespace Loan.Application.Models.LoanModels
{
    public class CreateLoanRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public LoanType LoanType { get; set; }
        public int DurationInMonths { get; set; }
    }
}

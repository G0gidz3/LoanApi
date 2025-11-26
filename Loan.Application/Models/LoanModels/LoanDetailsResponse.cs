using Loan.Domain.Enums;

namespace Loan.Application.Models.LoanModels
{
    public class LoanDetailsResponse
    {
        public int LoanId { get; set; }
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public LoanType LoanType { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public decimal TotalToPay { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

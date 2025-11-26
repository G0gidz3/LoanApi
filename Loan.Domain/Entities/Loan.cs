using Loan.Domain.Enums;

namespace Loan.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public LoanType LoanType { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}

using FluentValidation;

namespace Loan.Application.Models.LoanModels
{
    public class CreateLoanRequestValidator : AbstractValidator<CreateLoanRequest>
    {
        public CreateLoanRequestValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => (int)x.LoanType).GreaterThan(0).WithMessage("Loan type must be chosen.");
            RuleFor(x => x.DurationInMonths).GreaterThan(0).WithMessage("Duration must be at least 1 month.");
        }
    }
}

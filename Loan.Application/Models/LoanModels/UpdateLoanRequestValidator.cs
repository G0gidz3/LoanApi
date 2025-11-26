using FluentValidation;

namespace Loan.Application.Models.LoanModels
{
    public class UpdateLoanRequestValidator : AbstractValidator<UpdateLoanRequest>
    {
        public UpdateLoanRequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.DurationInMonths).GreaterThan(0);
        }
    }
}

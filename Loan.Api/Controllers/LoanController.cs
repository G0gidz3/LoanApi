using Loan.Application.Models.LoanModels;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Loan.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService loanService;

        public LoanController(ILoanService loanService)
        {
            this.loanService = loanService;
        }

        [Authorize(Roles = RoleType.User)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request, CancellationToken cancellationToken)
        {
            request.UserId = int.Parse(User.FindFirst(ClaimTypes.Name)!.Value);

            int loanId = await loanService.CreateLoanAsync(request, cancellationToken);
            return Ok(new { Message = "Loan successfully created", LoanId = loanId });
        }

        [Authorize(Roles = $"{RoleType.User},{RoleType.Accountant},{RoleType.Admin}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLoan(int id, CancellationToken cancellationToken)
        {
            var loan = await loanService.GetLoanByIdAsync(id, cancellationToken);
            return Ok(loan);
        }

        [Authorize(Roles = $"{RoleType.User},{RoleType.Accountant},{RoleType.Admin}")]
        [HttpGet]
        public async Task<IActionResult> GetLoans(CancellationToken cancellationToken)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Name)!.Value);

            var loans = await loanService.GetLoansByUserIdAsync(userId, cancellationToken);
            return Ok(loans);
        }

        [Authorize(Roles = $"{RoleType.Accountant},{RoleType.Admin}")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetLoansByUser(int userId, CancellationToken cancellationToken)
        {
            var loans = await loanService.GetLoansByUserIdAsync(userId, cancellationToken);
            return Ok(loans);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateLoan([FromBody] UpdateLoanRequest request, CancellationToken cancellationToken)
        {
            await loanService.UpdateLoanAsync(request, cancellationToken);
            return Ok(new { Message = "Loan successfully updated" });
        }

        [Authorize(Roles = $"{RoleType.Admin},{RoleType.Accountant}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id, CancellationToken cancellationToken)
        {
            await loanService.DeleteLoanByIdAsync(id, cancellationToken);
            return Ok(new { Message = "Loan successfully deleted" });
        }
    }
}

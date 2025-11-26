using Loan.Application.Models.UserModels;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Entities;
using Loan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Loan.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest, CancellationToken cancellationToken)
        {
            UserLoginResponse userLoginResponse = await userService.LoginAsync(userLoginRequest, cancellationToken);
            return Ok(userLoginResponse);
        }

        [Authorize(Roles = RoleType.Admin)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            int id = await userService.RegisterAsync(request, cancellationToken);
            return Ok(id);
        }

        [Authorize(Roles = $"{RoleType.Admin},{RoleType.Accountant}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            UserDetailResponse userDetailsResponse = await userService.GetUserDetailsAsync(id, cancellationToken);
            return Ok(userDetailsResponse);
        }

        [Authorize(Roles = $"{RoleType.Admin},{RoleType.Accountant}")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            List<UserDetailResponse> usersDetailsResponse = await userService.GetUserListDetailsAsync(cancellationToken);
            return Ok(usersDetailsResponse);
        }

        [Authorize(Roles = $"{RoleType.Admin},{RoleType.Accountant}")]
        [HttpPost("block/{id}")]
        public async Task<IActionResult> BlockUser(int id, [FromQuery] int timeToBlock, CancellationToken cancellationToken)
        {
            await userService.BlockUserAsync(id, timeToBlock, cancellationToken);
            return Ok();
        }
    }
}

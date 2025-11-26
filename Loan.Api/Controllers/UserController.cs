using Loan.Application.Models;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Loan.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IUserService userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest, CancellationToken cancellationToken)
        {
            UserLoginResponse userLoginResponse = await userService.LoginAsync(userLoginRequest, cancellationToken);
            if (userLoginResponse is null)
            {
                return BadRequest(new { message = "Wrong Credentials" });
            }

            return Ok(userLoginResponse);
        }

        //[Authorize(Roles = RoleType.Admin)]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromHeader(Name = "Authorization")] string token, [FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            int id = await userService.RegisterAsync(request, cancellationToken);

            if (id > 0)
                return Ok(id);
            return BadRequest(new { message = "User couldn't registered" });
        }
    }
}

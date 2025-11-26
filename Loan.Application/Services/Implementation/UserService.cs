using Loan.Application.Models;
using Loan.Application.Repositories.Abstraction;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Loan.Application.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IOptions<AppSettings> appSettings;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IOptions<AppSettings> appSettings)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.appSettings = appSettings;
        }
        public async Task<UserDetailResponse> GetUserDetailsAsync(int id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByIdAsync(id, cancellationToken);
            var userDetailResponse = new UserDetailResponse()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email = user.Email,
                Username = user.Username,
                IsBlocked = user.IsBlocked,
                Salary = user.Salary,
                Roles = user.UserRoles.Select(x => x.Role.RoleName).ToList()
            };

            return userDetailResponse;
        }

        public Task<List<UserDetailResponse>> GetUserListDetailsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetUserByUsernameAndPasswordAsync(request.Username, request.Password, cancellationToken);

            if (user == null)
                return null;

            UserLoginResponse userLogin = new UserLoginResponse()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Roles = user.UserRoles.Select(x => x.Role.RoleName).ToList(),
                Token = GenerateToken(user)
            };
            return userLogin;
        }

        public async Task<int> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Age = request.Age,
                Salary = request.Salary,
                Username = request.Username,
                Password = request.Password,
                IsBlocked = false
            };
            int userId = await userRepository.CreateUserAsync(user, cancellationToken);

            List<UserRole> userRoles = new List<UserRole>();
            foreach (string roleName in request.Roles)
            {
                Role role = await roleRepository.GetByRoleNameAsync(roleName, cancellationToken);

                if (role == null)
                {
                    await userRepository.DeleteUserByIdAsync(userId, cancellationToken);
                    throw new Exception($"Role with name {roleName} doesn't exists.");
                }

                userRoles.Add(new UserRole()
                {
                    UserId = userId,
                    RoleId = role.Id
                });
            }
            return userId;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Value.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Add roles
            foreach (var role in user.UserRoles.Select(r => r.Role.RoleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

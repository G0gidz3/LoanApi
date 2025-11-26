using Loan.Application.Models;
using Loan.Application.Models.UserModels;
using Loan.Application.Repositories.Abstraction;
using Loan.Application.Services.Abstraction;
using Loan.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loan.Application.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;
        private readonly IOptions<AppSettings> appSettings;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IOptions<AppSettings> appSettings)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.appSettings = appSettings;
        }

        public async Task BlockUserAsync(int id, int minutes, CancellationToken cancellationToken)
        {
            await userRepository.BlockUserByIdAsync(id, minutes, cancellationToken);
        }

        public async Task<UserDetailResponse> GetUserDetailsAsync(int id, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetUserByIdAsync(id, cancellationToken);
            if (user == null)
                throw new Exception($"User with id [{id}] doesn't exists.");

            UserDetailResponse userDetailResponse = new UserDetailResponse()
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

        public async Task<List<UserDetailResponse>> GetUserListDetailsAsync(CancellationToken cancellationToken)
        {
            List<User> users = await userRepository.GetUserlistAsync(cancellationToken);
            if (users == null)
                return new List<UserDetailResponse>();

            List<UserDetailResponse> userList = users.Select(user => new UserDetailResponse()
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
            }).ToList();
            return userList;
        }

        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken)
        {
            User user = await userRepository.GetUserByUsernameAndPasswordAsync(request.Username, request.Password, cancellationToken);

            if (user == null)
                throw new Exception("Wrong Credentials.");

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
                    throw new Exception($"Role with name [{roleName}] doesn't exists.");
                }

                userRoles.Add(new UserRole()
                {
                    UserId = userId,
                    RoleId = role.Id
                });
            }
            await userRoleRepository.CreateUserRolesAsync(userRoles, cancellationToken);
            return userId;
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Value.Secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}

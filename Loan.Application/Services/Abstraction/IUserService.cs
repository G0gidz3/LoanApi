using Loan.Application.Models.UserModels;

namespace Loan.Application.Services.Abstraction
{
    public interface IUserService
    {
        public Task<UserLoginResponse> LoginAsync(UserLoginRequest request, CancellationToken cancellationToken);
        public Task<UserDetailResponse> GetUserDetailsAsync(int id, CancellationToken cancellationToken);
        public Task BlockUserAsync(int id, int minutes, CancellationToken cancellationToken);
        public Task<List<UserDetailResponse>> GetUserListDetailsAsync(CancellationToken cancellationToken);
        public Task<int> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    }
}

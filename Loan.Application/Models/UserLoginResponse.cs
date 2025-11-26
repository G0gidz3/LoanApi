namespace Loan.Application.Models
{
    public class UserLoginResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}

namespace Loan.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}

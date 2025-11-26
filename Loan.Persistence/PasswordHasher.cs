using System.Security.Cryptography;
using System.Text;

namespace Loan.Persistence
{
    // სტატიკური კლასი პაროლთან მანიპულაციისთვის (სტატიკური რადგან არ არის საჭირო ინსტანსის შექმნა)
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}

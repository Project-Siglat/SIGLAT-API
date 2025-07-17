namespace Craftmatrix.Codex.org.Service
{

    public class PasswordService
    {
        // Hash a password
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verify a password against its hash
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}

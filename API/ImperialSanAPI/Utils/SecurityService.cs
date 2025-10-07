using System.Security.Cryptography;
using System.Text;

namespace ImperialSanAPI.Utils
{

    public class SecurityService
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 50000;

        static public byte[] HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым");

            using var hmac = new HMACSHA256();
            var salt = hmac.Key; // Используем HMAC для генерации соли

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(KeySize);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return hashBytes;
        }

        static public bool VerifyPassword(string password, byte[] storedHash)
        {
            if (storedHash == null || storedHash.Length != SaltSize + KeySize)
                return false;

            var salt = new byte[SaltSize];
            Array.Copy(storedHash, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(KeySize);
            
            for (int i = 0; i < 20; i++)
                if (storedHash[i + 16] != hash[i])
                    return false;

            return true;
        }

        static public bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.Length == 11 && phone.All(char.IsDigit);
        }
    }
}

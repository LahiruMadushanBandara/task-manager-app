using System.Security.Cryptography;
using System.Text;

namespace TaskManager.API.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password, string salt)
    {
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = SHA256.HashData(combined);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static bool Verify(string password, string salt, string storedHash) =>
        Hash(password, salt).Equals(storedHash, StringComparison.OrdinalIgnoreCase);
}

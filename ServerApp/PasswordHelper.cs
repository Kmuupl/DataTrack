using System.Security.Cryptography;
using System.Text;
class PasswordHelper
{
    public static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));
            
        }
        return sb.ToString();
    }
}
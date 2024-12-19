using System.Security.Cryptography;
using System.Text;

namespace Ngo_Project3_Api.Middleware
{
    public class MD5Encryption
    {
        public static string EncryptMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển đổi sang chuỗi Hexadecimal (chuỗi 32 ký tự)
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // "x2" để format thành 2 ký tự Hex
                }
                return sb.ToString();
            }
        }
    }
}

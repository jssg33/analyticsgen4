using System.Text;
using System.Numerics;
namespace Enterpriseservices
{

    public class Twofactor
    {
        public string GenerateKey()
        {
            // Get current UTC time in seconds since Unix epoch
            long seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Convert seconds to a byte array
            byte[] timeBytes = BitConverter.GetBytes(seconds);

            // Hash the byte array to get a consistent output
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(timeBytes);

                // Convert hash to base36 (alphanumeric) and take first 6 characters
                string base36 = Base36Encode(hash);
                return base36.Substring(0, 6).ToUpper();
            }
        }

        // Helper method to convert byte array to base36 string
        private string Base36Encode(byte[] bytes)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            BigInteger value = new BigInteger(bytes);
            StringBuilder result = new StringBuilder();

            while (value > 0)
            {
                int index = (int)(value % 36);
                result.Insert(0, chars[index]);
                value /= 36;
            }

            return result.ToString();
        }

    }
}

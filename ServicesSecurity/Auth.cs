/*using System.Security.Cryptography;
using System.Text;
using Enterprise.Models;

namespace EnterpriseServices
{
    //Moved to Globals.cs -> protected string Globalkey = "spirit";

    public class Auth
    {

        // CREATE A FUNCTION VALIDATE THAT RETURNS A BOOLEAN 1 FOR GOOD LOGIN, AND A TOKEN, OR 0 FOR FAILED LOGIN, AND A NULL VALUE FOR TOKEN
        public (int, string) Validate(int userid, int hashid, string userhashedpassword)
        {
            using (var context = new EnterpriseContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userid);
                if (user == null || string.IsNullOrEmpty(user.Hashedpassword))
                {
               
                	return (0, string.Empty); // 
                }

                // Compare hashed passwords (case-sensitive)
                bool successfull_login = user.Hashedpassword == userhashedpassword;

                if (!successfull_login)
                {
                	return (0, string.Empty); // 
                }
                else
                {
                    string sometoken = GenerateToken();
                    CreateSession(userid, sometoken);
                    return (1, sometoken);
                }
            } }

        //THIS FUNCTION WILL DEHASH THE PASSWORD

        private string dehashpassword(int hashid, string hashedpassword)
        {
            string someclearpassword = DecryptPassword(hashedpassword);
            return someclearpassword;
        }

        //THIS FUNCTION SIMPLY RETURNS A TOKEN OF 128 BIT IN LENGTH

        private string GenerateToken()
        {
            // 128 bits = 16 bytes
            byte[] tokenBytes = new byte[16];

            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            // Convert to a hexafloat string
            return BitConverter.ToString(tokenBytes).Replace("-", "").ToLower();
        }

        //THIS FUNCTION WILL OPEN A USERSESSION IF A SUCCESSFUL LOGIN HAS OCCURED

        private void CreateSession(int id, string token)
        {
            using (var context = new EnterpriseContext())
            {
                var session = new Usersession
                {
                    Userid = id,
                    Token = token,
                    Acknowledged = 0,
                    Actionpriority = 0,
                    Sessionstart = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                    Sessionend = null,
                    Sessionrecorded = 0,
                    Sessionrecordurl = null,
                    Sessiondescription = null,
                    Sessionusername = null,
                    Sessionemail = null,
                    Sessionfirstname = null,
                    Sessionlastname = null,
                    Sessionfullname = null,
                    Sessioncomplete = 0,
                    Twofactorkey = null,
                    Twofactorkeysmsdestination = null,
                    Twofactorkeyemaildestination = null,
                    Twofactorprovider = null,
                    Twofactorprovidertoken = null,
                    Twofactorproviderauthstring = null
                };

                context.Usersessions.Add(session);
                context.SaveChanges();
            }
        }

        

        // START WITH THE FIRST 

        private string EncryptPassword(string plainText)
        {
            string key = Globals.Globalkey;
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.GenerateIV();
                var encryptor = aes.CreateEncryptor();

                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length); // prepend IV
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(inputBytes, 0, inputBytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // WE ARE GOING TO PUT THE FIRST HASH IN THE SOURCE CODE...

        private string DecryptPassword(string cipherText)
        {
            string key = Globals.Globalkey;
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = fullCipher.Take(16).ToArray();
                var decryptor = aes.CreateDecryptor();

                using (var ms = new MemoryStream(fullCipher.Skip(16).ToArray()))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }



    } }
*/
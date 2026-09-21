//PGP CIPHER
// JOHN S. STRITZINGER
// SUMMER 2026

using System;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace Services
{
    public class pgpencryption
    {
        // -------------------------------
        //  GENERIC CAESAR ENCRYPT
        // -------------------------------
        private static string CaesarEncrypt(string input, int offset)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder();

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int shifted = ((c - baseChar + offset) % 26) + baseChar;
                    sb.Append((char)shifted);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        // -------------------------------
        //  GENERIC CAESAR DECRYPT
        // -------------------------------
        private static string CaesarDecrypt(string input, int offset)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sb = new StringBuilder();

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int shifted = ((c - baseChar - offset + 26) % 26) + baseChar;
                    sb.Append((char)shifted);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        // -------------------------------
        //  FIXED OFFSETS: 3, 5, 7
        // -------------------------------
        public string Caesar3(string input) => CaesarEncrypt(input, 3);
        public string Caesar5(string input) => CaesarEncrypt(input, 5);
        public string Caesar7(string input) => CaesarEncrypt(input, 7);

        public string Caesar3Decrypt(string input) => CaesarDecrypt(input, 3);
        public string Caesar5Decrypt(string input) => CaesarDecrypt(input, 5);
        public string Caesar7Decrypt(string input) => CaesarDecrypt(input, 7);

        // -------------------------------
        //  CURRENT-DAY MUTATOR (FINAL VERSION)
        // -------------------------------
        public int GetCurrentDayMutator()
        {
            var now = DateTime.UtcNow;

            // If hour > 8, subtract 4 hours (handles CST/PST drift)
            // Otherwise subtract 1 hour (normal rollover protection)
            var adjusted = now.Hour > 8
                ? now.AddHours(-4)
                : now.AddHours(-1);

            // Use adjusted day-of-month (1–31) as Caesar offset
            return adjusted.Day;
        }

        // -------------------------------
        //  GET CIPHER FROM USERSESSION
        // -------------------------------
        private string GetCipherFromUserSession(int sessionId)
        {
            using (var context = new EnterpriseContext())
            {
                var session = context.Usersessions
                    .FirstOrDefault(s => s.Id == sessionId);

                if (session == null)
                    throw new Exception("Usersession not found.");

                if (string.IsNullOrWhiteSpace(session.Targetcipher))
                    throw new Exception("Usersession has no Targetcipher.");

                return session.Targetcipher; // e.g. "caesar,3"
            }
        }

        // -------------------------------
        //  ADAPTABLE DISPATCH METHOD
        // -------------------------------
        public string Decrypt(string cipher, string encryptedValue, int? sessionId = null)
        {
            // CASE 1: cipher is null → use current-day mutator
            if (string.IsNullOrWhiteSpace(cipher))
            {
                int offset = GetCurrentDayMutator();
                return CaesarDecrypt(encryptedValue, offset);
            }

            // CASE 2: cipher == "usersession"
            if (cipher.ToLower() == "usersession")
            {
                if (sessionId == null)
                    throw new Exception("SessionId required for usersession cipher.");

                string actualCipher = GetCipherFromUserSession(sessionId.Value);

                return Decrypt(actualCipher, encryptedValue); // recursive dispatch
            }

            // CASE 3: static Caesar
            switch (cipher.ToLower())
            {
                case "caesar,3":
                    return Caesar3Decrypt(encryptedValue);

                case "caesar,5":
                    return Caesar5Decrypt(encryptedValue);

                case "caesar,7":
                    return Caesar7Decrypt(encryptedValue);

                default:
                    throw new Exception("Unsupported cipher type.");
            }
        }

        // -------------------------------
        //  ENCAPSULATE JSON INSIDE JSON
        // -------------------------------
        public string EncapsulateJson(object jsonObject)
        {
            var wrapper = new
            {
                timestamp = DateTime.UtcNow,
                payload = jsonObject
            };

            return JsonSerializer.Serialize(wrapper);
        }

        // -------------------------------
        //  ENCRYPT FULL JSON PAYLOAD
        // -------------------------------
        public string EncryptEncapsulatedJson(object jsonObject, int offset)
        {
            string encapsulated = EncapsulateJson(jsonObject);
            return CaesarEncrypt(encapsulated, offset);
        }
    }
}

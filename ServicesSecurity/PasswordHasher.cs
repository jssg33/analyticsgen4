//JOHN S. STRITZINGER
//CS547G -> FALL 2025
//ADDED FUNCTIONS TO ENCRYPT ALL PLAIN PASSWORDS SET VIA THE MANAGER TO ENCRYPTED FORM.
//ALTERNATIVELY, THE AUTH CONTROLLER NEEDS TO BE CREATED TO ADD A USER.
//A POST TO USER API WILL WORK, BUT IT WONT BE ABLE TO LOGIN WITHOUT THE BCRYPT FUNCTION.
//WE ARE GOING TO ADD AN ACTIONS CONTROLLER TO RUN THIS FROM A CALL.


using Enterprise.Models;
using Microsoft.EntityFrameworkCore;
namespace Enterpriseservices;

public class MyPasswords
{
    public static async Task HashAllUserPasswordsAsync()
    {
            using (var context = new EnterpriseContext())
            {
                // Clear out any "string" placeholders
                var badUsers = context.Users.Where(u => u.Hashedpassword == "string").ToList();
                foreach (var u in badUsers)
                {
                    u.Hashedpassword = null;
                }
                await context.SaveChangesAsync();

                // Now loop through and hash
                var users = context.Users.ToList();
                foreach (var user in users)
                {
                    if (!string.IsNullOrEmpty(user.Plainpassword) && string.IsNullOrEmpty(user.Hashedpassword))
                    {
                        user.Hashedpassword = BCrypt.Net.BCrypt.HashPassword(user.Plainpassword);
                    }
                }

                context.SaveChanges();

                // Verification
                var updatedUsers = context.Users
                    .Select(u => new { u.Id, u.Plainpassword, u.Hashedpassword })
                    .ToList();

                foreach (var u in updatedUsers)
                {
                    Console.WriteLine($"Id: {u.Id}, Plain: {u.Plainpassword}, Hashed: {u.Hashedpassword}");
                }
            }
    }
}

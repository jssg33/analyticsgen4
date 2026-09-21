using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using Enterprise.Models;
using NuGet.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Services;
using EnterpriseServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
namespace somecontrollers.Controllers;

public enum Roles
{
    admin,
    registered,
    guest
}

public static class Auth
{
    private static readonly string UsersFilePath = Path.Combine(AppContext.BaseDirectory, "Controllers", "Auth", "userList.json");
    private static readonly string CredentialsFilePath = Path.Combine(AppContext.BaseDirectory, "Controllers", "Auth", "userCredential.json");

    //  Using HttpContext because the current JSON based setup requires reading and writing to JSON files in memory
    //  Since we are not using a database yet, we load entire JSON lists into memory and process them in the API
    //  Once the UserCred table is set up in a database, this will connect to EnterpriseContext for real time access
    //  JSON storage requires reading entire files before finding user data

    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Auth").WithTags("Authentication");

        //  Login Route
        group.MapPost("/loginLocal", async (LoginRequest request, IConfiguration config) =>
        {
            var users = await LoadUsersFromJson();
            var user = users.FirstOrDefault(u => u.Username!.ToLower() == request.Username.ToLower());

            if (user == null)
                return Results.BadRequest("User not found.");

            var credentials = await LoadCredentialsFromJson();
            var userCredential = credentials.FirstOrDefault(c => c.UserId == user.Id);
            if (userCredential == null)
                return Results.BadRequest("User credentials not found.");

            //  Verify password using bcrypt
            bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.PlainPassword, userCredential.EncryptedPassword);
            if (!passwordMatches)
                return Results.BadRequest("Password mismatch.");

            var token = GenerateJwtToken(user, config);

            return Results.Ok(new
            {
                userId = user.Id,
                userEmail = user.Email,
                userFullName = user.Fullname,
                userUsername = user.Username,
                userRole = user.Role,
                token
            });
        })
        .WithName("loginUserLocal")
        .WithOpenApi();

        // ADDED ENCRYPTED LOGIN ON 07/22/2026 - JSS
group.MapPost("/login", (LoginRequest request, IConfiguration config) =>
{
    using (var context = new EnterpriseContext())
    {
        var user = context.Users
            .FirstOrDefault(u => u.Username!.ToLower() == request.Username.ToLower());

        if (user == null)
            return Results.BadRequest("User not found.");

        // Verify password using bcrypt
        bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.PlainPassword, user.Hashedpassword);
        if (!passwordMatches)
            return Results.BadRequest("Password mismatch.");

        // Generate JWT token
        var token = GenerateJwtToken(user, config);

        // Default cipher for new sessions
        var systemcipher = "caesar,7";

        // Create a new usersession
        var session = new Usersession
        {
            Userid = user.Id,
            Token = token,
            GoogleToken = null,
            Sessionstart = DateTime.UtcNow.ToString("o"),
            Sessionusername = user.Username,
            Sessionemail = user.Email,
            Sessionfirstname = user.Firstname,
            Sessionlastname = user.Lastname,
            Sessionfullname = user.Fullname,
            Useridasstring = user.Id.ToString(),
            Targetcipher = systemcipher
        };

        context.Usersessions.Add(session);
        context.SaveChanges();

        // Return login payload + cipher
        return Results.Ok(new
        {
            userId = user.Id,
            userFirstname = user.Firstname,
            userLastname = user.Lastname,
            userUsername = user.Username,
            userEmail = user.Email,
            IsEmployee = user.Employee,
            EmployeeId = user.Employeeid,
            userMicrosoftId = user.Microsoftid,
            userNcrId = user.Ncrid,
            userOracleId = user.Oracleid,
            userAzureId = user.Azureid,
            userJid = user.Jid,
            userProfileUrl = user.Profileurl,
            userRole = user.Role,
            userFullName = user.Fullname,
            userCompany = user.Companyid,
            userBtn = user.Btn,
            userIsCertified = user.Iscertified,
            Date = DateOnly.FromDateTime(DateTime.Now),
            token = token,
            sessionId = session.Id,
            Targetcipher = systemcipher
        });
    }
})
.WithName("loginUser")
.WithOpenApi();



        //  Signup Route
        group.MapPost("/signupLocal", async (SignupRequest request) =>
        {
            var users = await LoadUsersFromJson();

            if (users.Any(u => u.Email?.ToLower() == request.Email.ToLower()))
                return Results.BadRequest("Email is already registered.");

            if (users.Any(u => u.Username?.ToLower() == request.Username.ToLower()))
                return Results.BadRequest("Username is already in use.");

            var newUserId = users.Any() ? users.Max(c => c.Id) + 1 : 0;

            //  Hash the password with bcrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PlainPassword);

            var newUser = new User
            {
                Id = newUserId,
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Username = request.Username,
                Email = request.Email,
                Fullname = $"{request.Firstname} {request.Lastname}",
                Role = Roles.registered.ToString(),
                Hashedpassword = hashedPassword,
                Passwordtype = 1,
                Profileurl = "",
                Employee = 0,
                Employeeid = "",
                Microsoftid = "",
                Ncrid = "",
                Oracleid = "",
                Azureid = "",
                Plainpassword = "", // Not really needed/maybe shouldn't put their plain password in the databse?
                Jid = null,
                Companyid = null,
                Resettoken = null,
                Resettokenexpiration = null
            };

            users.Add(newUser);
            await SaveUsersToJson(users);

            var credentials = await LoadCredentialsFromJson();
            var newCredId = credentials.Any() ? credentials.Max(c => c.Id) + 1 : 1;

            var newCredential = new UserCred
            {
                Id = newCredId,
                UserId = newUserId,
                EncryptedPassword = hashedPassword // Same as what's in User
            };

            credentials.Add(newCredential);
            await SaveCredentialsToJson(credentials);

            return Results.Created($"/api/auth/{newUser.Id}", new { message = "User registered successfully." });
        })
        .WithName("signupUserLocal")
        .WithOpenApi();

        group.MapPost("/signup", async (SignupRequest request) =>
        {
            using (var context = new EnterpriseContext())
            {
                // Check for existing email or username
                if (context.Users.Any(u => u.Email!.ToLower() == request.Email.ToLower()))
                    return Results.BadRequest("Email is already registered.");

                if (context.Users.Any(u => u.Username!.ToLower() == request.Username.ToLower()))
                    return Results.BadRequest("Username is already in use.");

                // Hash the password securely
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PlainPassword);

                var newUser = new User
                {
                    Firstname = request.Firstname,
                    Lastname = request.Lastname,
                    Username = request.Username,
                    Email = request.Email,
                    Fullname = $"{request.Firstname} {request.Lastname}",
                    Role = Roles.registered.ToString(),
                    Hashedpassword = hashedPassword,
                    Passwordtype = 1,
                    Profileurl = "",
                    Employee = 0,
                    Employeeid = "",
                    Microsoftid = "",
                    Ncrid = "",
                    Oracleid = "",
                    Azureid = "",
                    Plainpassword = "", // Leave blank or null
                    Jid = null,
                    Companyid = null,
                    Resettoken = null,
                    Resettokenexpiration = null
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();
                // Combine the message and the user data into one object
            /* Optional Complex Response Payload. Issue Mobile Registration for Google Users needs to keep the UserIDs which are accurate.
            var response = new 
            {
            message = "User registered successfully.",
            user = new 
            {
                newUser.Id,
                newUser.Username,
                newUser.Email,
                newUser.Fullname,
                newUser.Role
            }
            };

            return Results.Created($"/api/auth/{newUser.Id}", response);
            */        

                return Results.Created($"/api/auth/{newUser.Id}", newUser);
            }
        })
        .WithName("signupUser")
        .WithOpenApi();

        //  Forgot Password Route
        group.MapPost("/forgotPasswordLocal", async (ForgotPasswordRequest request, ServiceCipherSupportsService serviceCipherSupportsService, IConfiguration config) =>
        {
            var users = await LoadUsersFromJson();
            var user = users.FirstOrDefault(u => u.Email?.ToLower() == request.Email.ToLower());

            if (user == null)
                return Results.NotFound("User not found (JSON).");

            var resetToken = Guid.NewGuid().ToString();
            var resetTokenExpiration = DateTime.UtcNow.AddHours(1);

            user.Resettoken = resetToken;
            user.Resettokenexpiration = resetTokenExpiration;

            await SaveUsersToJson(users);

            var resetLink = $"{config["FrontendUrl"]}/ResetPassword?token={resetToken}";

            var message = new
            {
                email = user.Email,
                subject = "Reset Your Password (JSON)",
                body = $"Click the link to reset your password: {resetLink}"
            };

            await serviceCipherSupportsService.SendMessageAsync(JsonConvert.SerializeObject(message));

            return Results.Ok(new { message = "Reset link sent to your email (JSON)." });
        })
        .WithName("forgotPasswordLocal")
        .WithOpenApi();

        group.MapPost("/forgotPassword", async (ForgotPasswordRequest request, ServiceCipherSupportsService serviceCipherSupportsService, IConfiguration config) =>
        {
            using (var context = new EnterpriseContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == request.Email.ToLower());

                if (user == null)
                    return Results.NotFound("User not found (DB).");

                var resetToken = Guid.NewGuid().ToString();
                var resetTokenExpiration = DateTime.UtcNow.AddHours(1);

                user.Resettoken = resetToken;
                user.Resettokenexpiration = resetTokenExpiration;

                await context.SaveChangesAsync();

                var resetLink = $"{config["FrontendUrl"]}/ResetPassword?token={resetToken}";

                var message = new
                {
                    email = user.Email,
                    subject = "Reset Your Password (DB)",
                    body = $"Click the link to reset your password: {resetLink}"
                };

                await serviceCipherSupportsService.SendMessageAsync(JsonConvert.SerializeObject(message));

                return Results.Ok(new { message = "Reset link sent to your email (DB)." });
            }
        }).WithName("forgotPassword")
        .WithOpenApi();

        group.MapPost("/resetPasswordProfile", async (ResetPasswordRequestProfile request, HttpContext httpContext, IConfiguration config) =>
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
                return Results.Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = config["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // DeCipherSupportg claims
                var claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }).ToList();
                // Console.WriteLine("All Claims:");
                // foreach (var claim in claims)
                // {
                //     Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                // }

                var username = claimsPrincipal.Claims.FirstOrDefault(c => 
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                // Console.WriteLine($"Username from claim: {username}");

                using (var context = new EnterpriseContext())
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                    
                    if (user == null)
                    {
                        // Console.WriteLine($"No user found for username: {username}");
                        return Results.BadRequest("User not found.");
                    }

                    // Verify current password
                    bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Hashedpassword);
                    if (!passwordMatches)
                        return Results.BadRequest("Current password is incorrect.");

                    // Update password
                    user.Hashedpassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                    await context.SaveChangesAsync();

                    return Results.Ok(new { message = "Password successfully updated.\n\nNew password: " + request.NewPassword});
                }
            }
            catch (Exception ex)
            {
                return Results.Unauthorized();
            }
        })
        .WithName("resetPasswordProfile")
        .WithOpenApi();


        group.MapPost("/resetPasswordLocal", async (ResetPasswordRequest request) =>
        {
            if (request.ResetToken == null)
                return Results.BadRequest("Token is null, user sholdn't be on this page.");

            var users = await LoadUsersFromJson();
            var user = users.FirstOrDefault(u =>
                u.Resettoken == request.ResetToken &&
                u.Resettokenexpiration > DateTime.UtcNow);

            if (user == null)
                return Results.BadRequest("Invalid or expired token.");

            user.Hashedpassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Resettoken = null;
            user.Resettokenexpiration = null;

            await SaveUsersToJson(users);

            return Results.Ok(new { message = "Password successfully reset (JSON)." });
        })
        .WithName("resetPasswordLocal")
        .WithOpenApi();

        group.MapPost("/resetPassword", async (ResetPasswordRequest request) =>
        {
            if (request.ResetToken == null)
                return Results.BadRequest("Token is null, user shouldn't be on this page.");

            using (var context = new EnterpriseContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u =>
                    u.Resettoken == request.ResetToken &&
                    u.Resettokenexpiration > DateTime.UtcNow);

                if (user == null)
                    return Results.BadRequest("Invalid or expired token.");

                user.Hashedpassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.Resettoken = null;
                user.Resettokenexpiration = null;

                await context.SaveChangesAsync();

                return Results.Ok(new { message = "Password successfully reset (DB)." });
            }
        })
        .WithName("resetPassword")
        .WithOpenApi();

    
        //TWO SCHOOLS OF THOUGHT FOR REGISTRATION - THE FIRST IS TO COMPLETE THE REGISTRATION AS FAST AS POSSIBLE WITHOUT WORRYING ABOUT SERIALIZATION DELAY AND A 2ND QUERY. (INSERT ONLY).
        // THIS REQUIRES TWO QUERIES TO THE DATABASE... THE FIRST IS AN INSERT, THEN A QUERY TO GET MY UID BACK. (2QUERIES).
        // THE PREVIOUS ADD TO THE REGISTRATION QUERY REQUIRES TWO QUERIES IN A SINGLE TRANSACTIONS (TSQL)... DEPENDING ON LOAD AND APP ONE OR THE OTHER MAY BE PREFERRED.
    
        // Get user ID by username route
        group.MapGet("/getidfromusername", async (string username) =>
        {
            if (string.IsNullOrWhiteSpace(username)) 
                return Results.BadRequest("Username cannot be empty.");

            var users = await LoadUsersFromJson();
            var user = users.FirstOrDefault(u => u.Username!.ToLower() == username.ToLower());

            if (user == null) 
                return Results.NotFound("User not found.");

            return Results.Ok(new { RecordId = user.Id });
        })
        .WithName("returnIDfromusername")
        .WithOpenApi();

//ADDED GOOGLE LOGIN ON 07/22/2026 - JSS
//UPDATED 08/29 CREATES AN ACCOUNT IF IT DOESNT EXIST
        
group.MapPost("/loginGoogle", async (GoogleSocialLoginRequest request, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.GoogleToken))
        return Results.BadRequest("Username and Google token are required.");

    var systemcipher = "caesar,7";

    using (var context = new EnterpriseContext())
    {
        // Try to find existing user
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username!.ToLower() == request.Username.ToLower());

        // If user does NOT exist → create one
        if (user == null)
        {
            user = new User
            {
                Username = request.Username,
                Email = request.Email,              // must be included in GoogleSocialLoginRequest
                Firstname = request.Firstname,      // must be included in GoogleSocialLoginRequest
                Lastname = request.Lastname,
                Fullname = $"{request.Firstname} {request.Lastname}",
                Role = "user",
                //CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Generate JWT
        var token = GenerateJwtToken(user, config);

        // Create session record
        var session = new Usersession
        {
            Userid = user.Id,
            Token = token,
            GoogleToken = request.GoogleToken,
            Acknowledged = 0,
            Actionpriority = 0,
            Sessionstart = DateTime.UtcNow.ToString("o"),
            Sessionend = null,
            Sessionrecorded = 0,
            Sessionusername = user.Username,
            Sessionemail = user.Email,
            Sessionfirstname = user.Firstname,
            Sessionlastname = user.Lastname,
            Sessionfullname = user.Fullname,
            Sessioncomplete = 0,
            Useridasstring = user.Id.ToString(),
            Targetcipher = systemcipher
        };

        context.Usersessions.Add(session);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            userId = user.Id,
            userFirstname = user.Firstname,
            userLastname = user.Lastname,
            userUsername = user.Username,
            userEmail = user.Email,
            userRole = user.Role,
            token = token,
            GoogleToken = request.GoogleToken,
            sessionId = session.Id
        });
    }
})
.WithName("loginGoogle")
.WithOpenApi();


// ADDED ENCRYPTED LOGIN ON 07/22/2026 - JSS
group.MapPost("/pgpLogin", async (PgpLoginRequest request, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(request.EncryptedUsername))
        return Results.BadRequest("EncryptedUsername is required.");

    if (string.IsNullOrWhiteSpace(request.EncryptedPassword))
        return Results.BadRequest("EncryptedPassword is required.");

    var pgp = new pgpencryption();

    // Determine which cipher to use
    string cipherToUse = request.Cipher;

    // If client says "usersession", load the last session's cipher
    if (!string.IsNullOrWhiteSpace(cipherToUse) && cipherToUse.ToLower() == "usersession")
    {
        using (var context = new EnterpriseContext())
        {
            var lastSession = context.Usersessions
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            if (lastSession == null || string.IsNullOrWhiteSpace(lastSession.Targetcipher))
                return Results.BadRequest("Usersession cipher not available.");

            cipherToUse = lastSession.Targetcipher;   // e.g. "caesar,3"
        }
    }

    string decryptedUsername;
    string decryptedPassword;

    try
    {
        decryptedUsername = pgp.Decrypt(cipherToUse, request.EncryptedUsername);
        decryptedPassword = pgp.Decrypt(cipherToUse, request.EncryptedPassword);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Cipher error: {ex.Message}");
    }

    using (var context = new EnterpriseContext())
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username!.ToLower() == decryptedUsername.ToLower());

        if (user == null)
            return Results.BadRequest("User not found.");

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(decryptedPassword, user.Hashedpassword);
        if (!passwordMatches)
            return Results.BadRequest("Password mismatch.");

        var token = GenerateJwtToken(user, config);

        var session = new Usersession
        {
            Userid = user.Id,
            Token = token,
            GoogleToken = null,
            Sessionstart = DateTime.UtcNow.ToString("o"),
            Sessionusername = user.Username,
            Sessionemail = user.Email,
            Sessionfirstname = user.Firstname,
            Sessionlastname = user.Lastname,
            Sessionfullname = user.Fullname,
            Useridasstring = user.Id.ToString(),
            Targetcipher = cipherToUse   // store the actual cipher used
        };

        context.Usersessions.Add(session);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            userId = user.Id,
            userFirstname = user.Firstname,
            userLastname = user.Lastname,
            userUsername = user.Username,
            userEmail = user.Email,
            userRole = user.Role,
            token = token,
            sessionId = session.Id,
            cipherUsed = string.IsNullOrWhiteSpace(request.Cipher)
                ? $"mutator:{pgp.GetCurrentDayMutator()}"
                : cipherToUse
        });
    }
})
.WithName("pgpLogin")
.WithOpenApi();

//ADDED THE FACEBOOK ENDPOINT TODAY 08/29/2026 USING THE GOOGLE LOGIN AS A TEMPLATE

group.MapPost("/loginFacebook", async (FacebookSocialLoginRequest request, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
    request.Email = $"{request.Firstname}{request.Lastname}@facebook.com"
    .Replace(" ", "")
    .ToLower();
    }

    var systemcipher = "caesar,7";

    using (var context = new EnterpriseContext())
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
        {
            user = new User
            {
                Username = request.Email,
                Email = request.Email,
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Fullname = $"{request.Firstname} {request.Lastname}",
                Role = "user",
                //CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        var token = GenerateJwtToken(user, config);

        var session = new Usersession
        {
            Userid = user.Id,
            Token = token,
            FacebookToken = request.FacebookToken,
            Sessionstart = DateTime.UtcNow.ToString("o"),
            Sessionusername = user.Username,
            Sessionemail = user.Email,
            Sessionfirstname = user.Firstname,
            Sessionlastname = user.Lastname,
            Sessionfullname = user.Fullname,
            Useridasstring = user.Id.ToString(),
            Targetcipher = systemcipher
        };

        context.Usersessions.Add(session);
        await context.SaveChangesAsync();

        return Results.Ok(new
        {
            userId = user.Id,
            userFirstname = user.Firstname,
            userLastname = user.Lastname,
            userUsername = user.Username,
            userEmail = user.Email,
            userRole = user.Role,
            token = token,
            FacebookToken = request.FacebookToken,
            sessionId = session.Id
        });
    }
})
.WithName("loginFacebook")
.WithOpenApi();

//ADDED THE MICROSOFT ENDPOINT TODAY 08/29/2026 USING THE GOOGLE LOGIN AS A TEMPLATE
//RECOMMENDED UPDATES FOR BETTER TOKEN EXTRACTION - GROK.COM ENHANCEMENT OF ORIGINAL
        
group.MapPost("/loginMicrosoft", async (
    MicrosoftSocialLoginRequest request,
    IConfiguration config,
    ILoggerFactory loggerFactory) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.MicrosoftToken))
        return Results.BadRequest("Email and Microsoft token are required.");

    var logger = loggerFactory.CreateLogger("loginMicrosoft");

    // ---------- 1. Validate the token against Entra ID ----------
    var principal = await ValidateEntraExternalIdTokenAsync(request.MicrosoftToken, config, logger);

    if (principal is null)
        return Results.Unauthorized();

    // Extract claims
    var tokenEmail = principal.FindFirst("preferred_username")?.Value
                  ?? principal.FindFirst(ClaimTypes.Email)?.Value
                  ?? principal.FindFirst("email")?.Value;

    var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value
                 ?? principal.FindFirst("given_name")?.Value;

    var familyName = principal.FindFirst(ClaimTypes.Surname)?.Value
                  ?? principal.FindFirst("family_name")?.Value;

    if (string.IsNullOrWhiteSpace(tokenEmail))
        return Results.BadRequest("Token does not contain an email claim.");

    // Optional but recommended: enforce that the email in the token matches the request
    if (!string.Equals(tokenEmail, request.Email, StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("Email in token does not match the requested email.");

    // ---------- 2. Find or create user ----------
    var systemCipher = config["SystemCipher"] ?? "caesar,7";

    await using var context = new EnterpriseContext();

    var user = await context.Users
        .FirstOrDefaultAsync(u => u.Email.ToLower() == tokenEmail.ToLower());

    if (user is null)
    {
        user = new User
        {
            Username = tokenEmail,
            Email = tokenEmail,
            Firstname = request.Firstname ?? givenName,
            Lastname = request.Lastname ?? familyName,
            Fullname = $"{request.Firstname ?? givenName} {request.Lastname ?? familyName}".Trim(),
            Role = "user"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    // ---------- 3. Create your own JWT + session ----------
    var token = GenerateJwtToken(user, config);

    var session = new Usersession
    {
        Userid = user.Id,
        Token = token,
        // MicrosoftToken = request.MicrosoftToken, // avoid storing the raw token if possible
        Sessionstart = DateTime.UtcNow.ToString("o"),
        Sessionusername = user.Username,
        Sessionemail = user.Email,
        Sessionfirstname = user.Firstname,
        Sessionlastname = user.Lastname,
        Sessionfullname = user.Fullname,
        Useridasstring = user.Id.ToString(),
        Targetcipher = systemCipher
    };

    context.Usersessions.Add(session);
    await context.SaveChangesAsync();

    return Results.Ok(new
    {
        userId = user.Id,
        userFirstname = user.Firstname,
        userLastname = user.Lastname,
        userUsername = user.Username,
        userEmail = user.Email,
        userRole = user.Role,
        token,
        sessionId = session.Id
    });
})
.WithName("loginMicrosoft")
.WithOpenApi();
        
}
    

    // File Handling Methods
    private static async Task<List<User>> LoadUsersFromJson()
    {
        if (!File.Exists(UsersFilePath)) return new List<User>();
        var jsonData = await File.ReadAllTextAsync(UsersFilePath);
        return JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
    }

    private static async Task SaveUsersToJson(List<User> users)
    {
        var jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);
        await File.WriteAllTextAsync(UsersFilePath, jsonData);
    }

    private static async Task<List<UserCred>> LoadCredentialsFromJson()
    {
        if (!File.Exists(CredentialsFilePath)) return new List<UserCred>();
        var jsonData = await File.ReadAllTextAsync(CredentialsFilePath);
        return JsonConvert.DeserializeObject<List<UserCred>>(jsonData) ?? new List<UserCred>();
    }

    private static async Task SaveCredentialsToJson(List<UserCred> credentials)
    {
        var jsonData = JsonConvert.SerializeObject(credentials, Formatting.Indented);
        await File.WriteAllTextAsync(CredentialsFilePath, jsonData);
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        //  In production should load from appSettings
        var jwtkey = config["Jwt:Key"];
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", user.Role ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // Token expiry
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

//RECOMENDED SUPPORT FOR MICROSOFT IDENT VALIDATION - GROK.COM

private static async Task<ClaimsPrincipal?> ValidateEntraExternalIdTokenAsync(
    string token,
    IConfiguration config,
    Microsoft.Extensions.Logging.ILogger logger)
{
    try
    {
        var authority = (config["AzureAd:Authority"] ?? "https://glocationinfo.ciamlogin.com").TrimEnd('/');
        var clientId = config["AzureAd:ClientId"];
        var audience = config["AzureAd:Audience"] ?? clientId;
        var tenantId = config["AzureAd:TenantId"] ?? "57836496-6e32-4a51-bf60-8e4c21bbf622";

        var metadataAddress = $"{authority}/{tenantId}/v2.0/.well-known/openid-configuration";

        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true });

        var openIdConfig = await configManager.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = openIdConfig.Issuer, // → https://57836496-6e32-4a51-bf60-8e4c21bbf622.ciamlogin.com/57836496-6e32-4a51-bf60-8e4c21bbf622/v2.0

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = openIdConfig.SigningKeys,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(token, validationParameters, out _);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Entra External ID token validation failed");
        return null;
    }
}
    
}

public class LoginRequest { public string Username { get; set; } = string.Empty; public string PlainPassword { get; set; } = string.Empty; }
public class SignupRequest { public string Firstname { get; set; } = string.Empty; public string Lastname { get; set; } = string.Empty; public string Username { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string PlainPassword { get; set; } = string.Empty; }
public class ForgotPasswordRequest { public string Email { get; set; } = string.Empty; }
public class ResetPasswordRequest { public string ResetToken { get; set; } = string.Empty; public string NewPassword { get; set; } = string.Empty; }
public class ResetPasswordRequestProfile { public string CurrentPassword { get; set; } = string.Empty; public string NewPassword { get; set; } = string.Empty; }
public class GoogleSocialLoginRequest { public string Username { get; set; } = string.Empty; public string Lastname { get; set; } = string.Empty; public string Firstname { get; set; } = string.Empty; public string Email { get; set; } = string.Empty;  public string GoogleToken { get; set; } = string.Empty; }
public class PgpLoginRequest { public string EncryptedUsername { get; set; } = string.Empty; public string EncryptedPassword { get; set; } = string.Empty; public string Cipher { get; set; } = string.Empty; }  // e.g. "caesar,3" }
public class FacebookSocialLoginRequest { public string Email { get; set; } = string.Empty; public string FacebookToken { get; set; } = string.Empty; public string Firstname { get; set; } = string.Empty; public string Lastname { get; set; } = string.Empty; }
public class MicrosoftSocialLoginRequest { public string Email { get; set; } = string.Empty; public string MicrosoftToken { get; set; } = string.Empty; public string Firstname { get; set; } = string.Empty; public string Lastname { get; set; } = string.Empty; }



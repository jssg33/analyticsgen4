namespace Enterpriseservices
{

    public class Authlogger
    {
        public static string? testapi;
        public static void loguser(string username, int hashid, string hashedpassword, string loginstatus)
        {
            var loguserstring = username + "," + hashid + "," + hashedpassword + "," + loginstatus;
            return;
        }
    }
}

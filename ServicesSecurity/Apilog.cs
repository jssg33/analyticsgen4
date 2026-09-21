using Enterprise.Models;

namespace EnterpriseServices;


public class ApiLogger
{
    public static string? testapi;

    public static void logapi(string apiname, string apinumber, string eptype, int hashid, string parameterlist, string apiresult)
    {
        using (var context = new EnterpriseContext())
        {
            var logEntry = new Apilog
            {
                Apiname = apiname,
                Apinumber = apinumber,
                Eptype = eptype,
                Hashid = hashid,
                Parameterlist = parameterlist,
                Apiresult = apiresult
            };

            context.Apilogs.Add(logEntry);
            context.SaveChanges();
            return;
        }

    }
}

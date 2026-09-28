/*  John S. Stritzinger
    09/27/2026
*/

using Enterprise.Models;
using System.Net;

namespace EnterpriseServices
{
    public class BackendWebServerAuditResult
    {
        public string? ServerName { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }

        public bool HttpValid { get; set; }
        public bool HttpsValid { get; set; }

        public int? HttpStatusCode { get; set; }
        public int? HttpsStatusCode { get; set; }

        public DateTime AuditDate { get; set; }
    }

    public static class BackendWebServerPoller
    {
        public static List<BackendWebServerAuditResult> PollBackendServers()
        {
            using var context = new EnterpriseContext();

            var servers = context.WebServers
                .Where(x => x.isProxySlave == true && x.Active)
                .OrderBy(x => x.ServerName)
                .ToList();

            var results = new List<BackendWebServerAuditResult>();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(handler);

            client.Timeout = TimeSpan.FromSeconds(10);

            foreach (var server in servers)
            {
                bool httpValid = false;
                bool httpsValid = false;

                int? httpStatusCode = null;
                int? httpsStatusCode = null;

                var host = !string.IsNullOrWhiteSpace(server.Hostname)
                    ? server.Hostname
                    : server.IpAddress;

                if (!string.IsNullOrWhiteSpace(host))
                {
                    try
                    {
                        var response = client
                            .GetAsync($"http://{host}")
                            .GetAwaiter()
                            .GetResult();

                        httpValid = true;
                        httpStatusCode = (int)response.StatusCode;
                    }
                    catch
                    {
                        httpValid = false;
                    }

                    try
                    {
                        var response = client
                            .GetAsync($"https://{host}")
                            .GetAwaiter()
                            .GetResult();

                        httpsValid = true;
                        httpsStatusCode = (int)response.StatusCode;
                    }
                    catch
                    {
                        httpsValid = false;
                    }
                }

                results.Add(new BackendWebServerAuditResult
                {
                    ServerName = server.ServerName,
                    Hostname = server.Hostname,
                    IpAddress = server.IpAddress,

                    HttpValid = httpValid,
                    HttpsValid = httpsValid,

                    HttpStatusCode = httpStatusCode,
                    HttpsStatusCode = httpsStatusCode,

                    AuditDate = DateTime.UtcNow
                });
            }

            return results;
        }
    }
}
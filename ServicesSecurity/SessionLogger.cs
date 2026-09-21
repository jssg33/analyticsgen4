using System;
using System.IO;
using System.Text.Json;
using Enterprise.Models;

namespace Enterpriseservices
{
    /// <summary>
    /// Provides a session logging utility that writes to file/console and persists to Sessionlog table.
    /// </summary>
    public class SessionsLogger
    {
        public string? testsession;

        /// <summary>
        /// Logs a session event with metadata and description (can embed JSON).
        /// </summary>
        public static void SessionLog(
            string username,
            int hashid,
            DateTime sessionstart,
            DateTime sessionend,
            string moduleid,
            string description)
        {
            // Build a log line
            var logsessionstring =
                $"{DateTime.UtcNow:o} | User={username} | HashId={hashid} | Start={sessionstart:o} | End={sessionend:o} | Module={moduleid} | Desc={description}";

            // Write to file
            var logFile = Path.Combine(AppContext.BaseDirectory, "sessionlog.txt");
            File.AppendAllText(logFile, logsessionstring + Environment.NewLine);

            // Write to console
            Console.WriteLine(logsessionstring);

            // Persist to Sessionlog table
            try
            {
                using var context = new EnterpriseContext();
                var sessionLog = new Sessionlog
                {
                    Username = username,
                    Hashid = hashid,
                    Sessionstart = sessionstart,
                    Sessionend = sessionend,
                    Moduleid = moduleid,
                    Description = description
                };

                context.Sessionlogs.Add(sessionLog);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write Sessionlog to DB: {ex.Message}");
            }
        }
    }
}

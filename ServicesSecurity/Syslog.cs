/*using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Enterprise.Services
{
    /// <summary>
    /// Provides methods to send syslog messages for cart processing results.
    /// </summary>
    public class SyslogService
    {
        private readonly string _syslogServer;
        private readonly int _syslogPort;

        /// <summary>
        /// Initializes a new SyslogService.
        /// </summary>
        /// <param name="syslogServer">Syslog server hostname or IP.</param>
        /// <param name="syslogPort">Syslog server port (default 514).</param>
        public SyslogService(string syslogServer = "localhost", int syslogPort = 514)
        {
            _syslogServer = syslogServer;
            _syslogPort = syslogPort;
        }

        /// <summary>
        /// Logs a failed cart post with the inbound DTO serialized as JSON.
        /// </summary>
        public void LogFailedCart(CGCompletedCartDto dto, string reason)
        {
            try
            {
                string payload = JsonSerializer.Serialize(dto);
                string message = $"Cart POST failed: {reason}. Payload: {payload}";
                SendSyslogMessage(message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Syslog logging failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a successful booking with the result serialized as JSON.
        /// </summary>
        public void LogSuccessfulBooking(CartProcessingResult result)
        {
            try
            {
                string payload = JsonSerializer.Serialize(result);
                string message = $"Booking succeeded. Result: {payload}";
                SendSyslogMessage(message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Syslog logging failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a raw syslog message to the configured server.
        /// </summary>
        private void SendSyslogMessage(string message)
        {
            using (var client = new UdpClient())
            {
                client.Connect(_syslogServer, _syslogPort);

                // Syslog format: <PRI>timestamp hostname appname pid message
                string syslogMessage =
                    $"<134>{DateTime.UtcNow:MMM dd HH:mm:ss} dirtbike-api CartService: {message}";
                byte[] data = Encoding.UTF8.GetBytes(syslogMessage);

                client.Send(data, data.Length);
            }
        }
    }
}
*/
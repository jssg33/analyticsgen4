using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace EnterpriseServices
{

  public class ServiceCipherSupportsService
  {
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceCipherSupportsService(IConfiguration configuration)
    {
      _connectionString = configuration["ServiceCipherSupports:Certs:ConnectionString"]!;
      _queueName = configuration["ServiceCipherSupports:Certs:QueueName"]!;
      _client = new ServiceBusClient(_connectionString);
      _sender = _client.CreateSender(_queueName);
    }

    public async Task SendMessageAsync(string message)
    {
      ServiceBusMessage CipherSupportsMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
      await _sender.SendMessageAsync(CipherSupportsMessage);
    }
  }
}

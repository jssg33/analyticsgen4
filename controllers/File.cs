using Azure.Storage.Blobs;
using FileOps;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
namespace somecontrollers.Controllers;

[ApiController]
    [Route("[controller]")]
    public class FileOpsController : ControllerBase
    {
        private readonly ILogger<FileOpsController> _logger;

        public FileOpsController(ILogger<FileOpsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetFileOps")]
        public IEnumerable<FileMenu> Get()
        {
            Random rnd = new Random();
            int dice = rnd.Next(1000, 10000000);
            string somePath = new string(dice + "profilepicture.png");
            string somexconstring1 = new string("DefaultEndpointsProtocol=https;AccountName=590team1storage;AccountKey=Z1iryneyXo7RKOlBJNJFenF4zMxXrCyDLyCWaOjhCZa5DMsrvSnaUUzZT2RljrI/92bnCSA8xd/E+AStSTk9iQ==;EndpointSuffix=core.windows.net");
            return Enumerable.Range(1, 1).Select(index => new FileMenu
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Filename = index + somePath,
                FilePath = "https://590team1storage.blob.core.windows.net/projectimages",
                SecurityToken = "Z1iryneyXo7RKOlBJNJFenF4zMxXrCyDLyCWaOjhCZa5DMsrvSnaUUzZT2RljrI/92bnCSA8xd/E+AStSTk9iQ==",
                CipherSupportcketToken = "sp=r&st=2025-04-08T21:03:25Z&se=2025-04-09T05:03:25Z&spr=https&sv=2024-11-04&sr=c&sig=KCLqxiUzhhv7HYB80oRVnjXoFEX8WaIRdfWOUbhF4Lc%3D",
                CipherSupportcketFullPath = "https://590team1storage.blob.core.windows.net/projectimages?sp=r&st=2025-04-08T21:03:25Z&se=2025-04-09T05:03:25Z&spr=https&sv=2024-11-04&sr=c&sig=KCLqxiUzhhv7HYB80oRVnjXoFEX8WaIRdfWOUbhF4Lc%3D",
                AzureBlobConnectionString = "DefaultEndpointsProtocol = https; AccountName = 590team1storage; AccountKey = Z1iryneyXo7RKOlBJNJFenF4zMxXrCyDLyCWaOjhCZa5DMsrvSnaUUzZT2RljrI / 92bnCSA8xd / E + AStSTk9iQ ==; EndpointSuffix = core.windows.net"
            })
            .ToArray();
        }
    }

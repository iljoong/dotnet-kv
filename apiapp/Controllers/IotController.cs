using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using apiapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;

namespace apiapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private EventHubClient eventHubClient;
        string EventHubConnectionString = "NO CONN";
        string EventHubName = "NO NAME";
        private bool SetRandomPartitionKey = false;

        public IotController(IConfiguration _config)
        {
            // print all config
            /*foreach(var conf in _config.AsEnumerable())
            {
                Console.WriteLine($"Trace: Config: {conf.Key}:{conf.Value}");
            }*/

            // get config
            EventHubConnectionString = _config["iot:eventhubconn"];
            EventHubName = _config["iot:eventhubname"];

            try
            {
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
                {
                    EntityPath = EventHubName
                };
                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine($"---Error: {ex.Message}");
            }

        }

        ~IotController()
        {
            eventHubClient.Close();
        }

        // GET api/iot
        [HttpGet]
        public ActionResult<string> Get()
        {
            return $"eventhubname={EventHubName}";
        }

        // GET status
        [HttpGet("/config/eventhub")]
        public ActionResult<string> Status()
        {
            string conn = Regex.Replace(EventHubConnectionString, "SharedAccessKey=.*$", "");

            return $"eventhubname={EventHubName}\neventhubconn={conn}\n";
        }

        // GET with path
        [HttpGet("{model}/{ver}")]
        public ActionResult<string> Get(string model, string ver)
        {
            return $"info of {model}, {ver}";
        }

        // GET with path + header
        [HttpGet("{ver}")]
        public ActionResult<string> Get(string ver)
        {
            string model = this.Request.Headers["model"];

            return $"info of {model}, {ver}";
        }

        // POST
        [HttpPost]
        public async Task Post([FromBody] Device dev)
        {

            string message = MakeMessage(dev);

            await SendMessagesToEventHub(message);

            Console.WriteLine($"----{message}");
        }

        // PUT api/iot/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE api/iot/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string MakeMessage(Device dev)
        {
            string timestamp = dev.Timestamp != null
                ? dev.Timestamp.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'")
                : DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

            string temp = $"{timestamp}, {dev.Model}, {dev.Ver:0.000}, {dev.Payload}";

            return temp;
        }

        private async Task SendMessagesToEventHub(string message)
        {
            try
            {
                if (SetRandomPartitionKey)
                {
                    var pKey = Guid.NewGuid().ToString();
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)), pKey);
                    Debug.WriteLine($"Sent message: '{message}' Partition Key: '{pKey}'");
                }
                else
                {
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    Debug.WriteLine($"Sent message: '{message}'");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }
    }
}

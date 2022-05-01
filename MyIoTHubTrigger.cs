using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Assessment.Function
{
    public class TemperatureItem
  {
    [JsonProperty("id")]
    public string Id {get; set;}
    public double Temperature {get; set;}
    public double Humidity {get; set;}
  }

    public class MyIoTHubTrigger
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("MyIoTHubTrigger")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "AzureEventHubConnectionString")] EventData message,
        [CosmosDB(databaseName: "IoTData",
                                 collectionName: "Temperatures",
                                 ConnectionStringSetting = "cosmosDBConnectionString")] out TemperatureItem output,
                       ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");

var jsonBody = Encoding.UTF8.GetString(message.Body);
dynamic data = JsonConvert.DeserializeObject(jsonBody);
double temperature = data.temperature;
double humidity = data.humidity;

output = new TemperatureItem
 {
    Temperature = temperature,
    Humidity = humidity
};



        }
              [FunctionName("GetTemperature")]
      public static IActionResult GetTemperature(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "temperature/")] HttpRequest req,
        [CosmosDB(databaseName: "IoTData",
                  collectionName: "Temperatures",
                  ConnectionStringSetting = "cosmosDBConnectionString",
                      SqlQuery = "SELECT * FROM c")] IEnumerable< TemperatureItem> temperatureItem,
                  ILogger log)
      {
        return new OkObjectResult(temperatureItem);
      }

    }
}
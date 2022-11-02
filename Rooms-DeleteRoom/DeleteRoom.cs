using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Communication.Rooms;

namespace ACSUIBackend
{
    public static class DeleteRoom
    {
        [FunctionName("Rooms-DeleteRoom")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // do some validation if the values exist
            // if we fail return a bad code
            string roomId = data?.roomId;

            var response = await client.DeleteRoomAsync(roomId);

            return new OkObjectResult(response.ReasonPhrase);

        }
    }
}

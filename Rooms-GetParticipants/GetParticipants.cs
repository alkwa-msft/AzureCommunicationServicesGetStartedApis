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
using Azure;

namespace ACSUIBackend
{
    public static class GetParticipants
    {
        [FunctionName("Rooms-GetParticipants")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string roomId = data?.roomId;

            // do validation of the parameters for this function
            // if we fail return a bad code

            // wrap this in a try/catch and send a bad code if it fails
            Response<ParticipantsCollection> response = await client.GetParticipantsAsync(roomId);

			return new OkObjectResult(response.Value);
        }
    }
}

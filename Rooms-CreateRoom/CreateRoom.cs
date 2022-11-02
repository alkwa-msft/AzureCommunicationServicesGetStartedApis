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
    public static class CreateRoom
    {
        [FunctionName("Rooms-CreateRoom")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string validFromStr = data?.validFrom ?? DateTime.Now.ToString();
            string validUntilStr = data?.validUntil ?? DateTime.Now.AddDays(1).ToString();

            // do some validation if the values exist
            // if we fail return a bad code
            DateTime validFrom = DateTime.Parse(validFromStr);
			DateTime validUntil = DateTime.Parse(validUntilStr);

			// do validation of the parameters for this function
			// if we fail return a bad code
			
            // wrap this in a try/catch and send a bad code if it fails
            Response<CommunicationRoom> response = await client.CreateRoomAsync(validFrom: validFrom, validUntil: validUntil, roomJoinPolicy: RoomJoinPolicy.InviteOnly);

            
			return new OkObjectResult(response.Value);
        }
    }
}

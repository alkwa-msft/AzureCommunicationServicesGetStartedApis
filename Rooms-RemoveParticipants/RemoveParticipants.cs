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
using Azure.Communication.Rooms.Models;
using Azure.Communication;
using System.Collections.Generic;
using Azure;

namespace ACSUIBackend
{
    public static class RemoveParticipants
    {
        [FunctionName("Rooms-RemoveParticipants")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string acsUserId = data?.acsUserId;
            string roomId = data?.roomId;

            CommunicationUserIdentifier identifier = new CommunicationUserIdentifier(acsUserId);

			// wrap this in a try/catch and send a bad code if it fails
	        var response = await client.RemoveParticipantsAsync(roomId, new List<CommunicationIdentifier> { identifier });

            // wrap this in a try/catch and send a bad code if it fails
            Response<CommunicationRoom> getRoomResponse = await client.GetRoomAsync(roomId);

            return new OkObjectResult(getRoomResponse.Value);
        }
    }
}

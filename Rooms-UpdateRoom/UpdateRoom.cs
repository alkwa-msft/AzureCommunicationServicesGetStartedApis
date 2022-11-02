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
    public static class UpdateRoom
    {
        [FunctionName("Rooms-UpdateRoom")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string roomId = data?.roomId;
            string validFromStr = data?.validFrom ?? null;
            string validUntilStr = data?.validUntil ?? null;
            string roomJoinPolicyStr = data?.roomJoinPolicy ?? null;

            // do some validation if the values exist
            // if we fail return a bad code
            DateTime validFrom = DateTime.Parse(validFromStr);
			DateTime validUntil = DateTime.Parse(validUntilStr);
            RoomJoinPolicy roomJoinPolicy = getRoomJoinPolicyFromStr(roomJoinPolicyStr);

            // do validation of the parameters for this function
            // if we fail return a bad code

            // wrap this in a try/catch and send a bad code if it fails
            Response<CommunicationRoom> response = await client.UpdateRoomAsync(roomId, validFrom, validUntil, roomJoinPolicy);

			return new OkObjectResult(response.Value);
        }

        public static RoomJoinPolicy getRoomJoinPolicyFromStr(string policyStr) {
            if (policyStr == RoomJoinPolicy.CommunicationServiceUsers.ToString()) {
                return RoomJoinPolicy.CommunicationServiceUsers;
            }
            return RoomJoinPolicy.InviteOnly;
        }
    }
}

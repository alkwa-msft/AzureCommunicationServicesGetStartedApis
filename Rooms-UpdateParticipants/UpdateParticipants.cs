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
using Azure.Communication;
using System.Collections.Generic;

namespace ACSUIBackend
{
	public static class UpdateParticipant
	{
		[FunctionName("Rooms-UpdateParticipant")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");
			RoomsClient client = new RoomsClient(Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString"));

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);

			string acsUserId = data?.acsUserId;
			string roomId = data?.roomId;
			string role = data?.role;

			CommunicationUserIdentifier identifier = new CommunicationUserIdentifier(acsUserId);
			RoomParticipant participant = new RoomParticipant(identifier);

			// wrap this in a try/catch and send a bad code if it fails
			var response = await client.UpdateParticipantsAsync(roomId, new List<RoomParticipant> { participant });

			return new OkObjectResult(response);
		}

		private static RoleType getRoleFromStr(string role)
		{
			if (role == "Consumer")
			{
				return RoleType.Consumer;
			}
			else if (role == "Attendee")
			{
				return RoleType.Attendee;
			}

			return RoleType.Presenter;
		}
	}
}

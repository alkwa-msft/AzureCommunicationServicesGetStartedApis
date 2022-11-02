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
	public static class AddParticipants
	{
		[FunctionName("Rooms-AddParticipants")]
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
			string roleStr = data?.role;

			CommunicationUserIdentifier identifier = new CommunicationUserIdentifier(acsUserId);

			RoomParticipant participant;

			if (roleStr == null) {
				RoleType role = getRoleFromStr(roleStr);
				participant = new RoomParticipant(identifier, role);
			}
			else {
				participant = new RoomParticipant(identifier);
			}

			// wrap this in a try/catch and send a bad code if it fails
			var response = await client.AddParticipantsAsync(roomId, new List<RoomParticipant> { participant });
			response.Content.ToString();
			var rawJson = await new StreamReader(response.ContentStream).ReadToEndAsync();
			string test = JsonConvert.SerializeObject(response.Value);
			

			return new OkObjectResult(test);
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

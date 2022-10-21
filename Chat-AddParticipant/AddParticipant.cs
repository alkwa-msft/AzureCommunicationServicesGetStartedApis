using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Communication.Identity;
using System.Collections.Generic;
using Azure;
using Azure.Core;
using Azure.Communication.Chat;
using Azure.Communication;

namespace AzureCommunicationServicesGetStartedApis
{
	public static class AddParticipant
    {
        [FunctionName("Chat-AddParticipant")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string resourceConnectionStr = Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString");
            string adminUserId = Environment.GetEnvironmentVariable("adminUserId");
            string endpointUrl = Environment.GetEnvironmentVariable("endpointUrl");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string acsUserId = data?.acsUserId;

            if (acsUserId == "" || acsUserId == null)
            {
                return new BadRequestObjectResult("[Chat-AddParticipant] - acsUserId cannot be null or empty");
            }

            string displayName = data?.displayName;

             if (displayName == "" || displayName == null)
            {
                return new BadRequestObjectResult("[Chat-AddParticipant] - displayName cannot be null or empty");
            }

            string threadId = data?.threadId;

            if (threadId == "" || threadId == null)
            {
                return new BadRequestObjectResult("[Chat-AddParticipant] - threadId cannot be null or empty");
            }

            CommunicationIdentityClient client = new CommunicationIdentityClient(resourceConnectionStr);
            Response<AccessToken> tokenResponse = await client.GetTokenAsync(new CommunicationUserIdentifier(adminUserId), new List<CommunicationTokenScope> { CommunicationTokenScope.Chat });
            ChatClient chatClient = new ChatClient(new Uri(endpointUrl), new CommunicationTokenCredential(tokenResponse.Value.Token));
            ChatThreadClient threadClient = chatClient.GetChatThreadClient(threadId);

            try
			{
                ChatParticipant chatParticipant = new ChatParticipant(new CommunicationUserIdentifier(acsUserId));
                chatParticipant.DisplayName = displayName;
                Response addParticipantResponse = await threadClient.AddParticipantAsync(chatParticipant);
                return new OkObjectResult(addParticipantResponse.Status);
            }
            catch(RequestFailedException ex)
			{
                return new BadRequestObjectResult(ex.Message);
			} 
        }
    }
}

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
	public static class CreateChatThread
    {
        [FunctionName("Chat-CreateChatThread")]
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
            string topic = data?.topic;

            if (topic == "" || topic == null)
            {
                return new BadRequestObjectResult("[Chat-CreateChatThread] - topic cannot be null or empty");
            }

            CommunicationIdentityClient client = new CommunicationIdentityClient(resourceConnectionStr);
            Response<AccessToken> tokenResponse = await client.GetTokenAsync(new CommunicationUserIdentifier(adminUserId), new List<CommunicationTokenScope> { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP});
            ChatClient chatClient = new ChatClient(new Uri(endpointUrl), new CommunicationTokenCredential(tokenResponse.Value.Token));

            var chatParticipant = new ChatParticipant(identifier: new CommunicationUserIdentifier(id: adminUserId))
            {
                DisplayName = ""
            };

            try
            {
                // Bug in the C# code where you need to explicitly add someone when you are creating a thread.
                // To have parity with JS SDK we are going to add the user that created the thread explicitly.
                Response<CreateChatThreadResult> threadResponse = await chatClient.CreateChatThreadAsync(topic, participants: new[] { chatParticipant });
                return new OkObjectResult(threadResponse.Value);
            }
            catch(RequestFailedException ex)
			{
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

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

namespace AzureCommunicationServicesGetStartedApis
{
    public static class CreateUserAndToken
    {
        [FunctionName("Identity-CreateUserAndToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string resourceConnectionStr = Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString");
            CommunicationIdentityClient client = new CommunicationIdentityClient(resourceConnectionStr);

            try
            {
                Response<CommunicationUserIdentifierAndToken> response = await client.CreateUserAndTokenAsync(new List<CommunicationTokenScope> { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP });
                return new OkObjectResult(response.Value);
            }
            catch (RequestFailedException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

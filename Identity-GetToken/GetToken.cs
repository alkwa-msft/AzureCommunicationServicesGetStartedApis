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
using Microsoft.Azure.WebJobs.Host;

namespace AzureCommunicationServicesGetStartedApis
{
	public static class GetToken
    {
        [FunctionName("Identity-GetToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string resourceConnectionStr = Environment.GetEnvironmentVariable("AzureCommunicationServicesResourceConnectionString");
            CommunicationIdentityClient client = new CommunicationIdentityClient(resourceConnectionStr);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string acsUserId = data?.acsUserId;

            if (acsUserId == "" || acsUserId == null)
			{
                return new BadRequestObjectResult("[Identity-GetToken] - acsUserId cannot be null or empty");
			}
 
            try
            {
                Response<AccessToken> response = await client.GetTokenAsync(new Azure.Communication.CommunicationUserIdentifier(acsUserId), new List<CommunicationTokenScope> { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP });
                return new OkObjectResult(response.Value);
            }
            catch (RequestFailedException ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

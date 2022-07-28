# Azure Communication Services: Get Started Apis

This library of azure functions is designed to help developers get started quickly to create end to end experiences with their client side applications.

I mostly build client-side experiences but I found that I keep needing to have a set of backend functionality to prototype some of my communication experiments. I found I was creating these functions constantly and deploying them all of the time. Perhaps if I had this issue then others did too?

I suggest using this project to
- 🚗 bootstrap yourself quickly in a hackathon/weekend project
- 🤓 learn how these functions work with the postman localhost collection
- 🐱‍🏍 try out new things and create your own forks with your own functions

I do not suggest to use this project to
- create production-ready applications
- judge my ability wrote code (I wrote this during a hackathon too)

## Goal:
- ✅ Have a common backend to help others get started
- ⬜ Be able to deploy this backend quickly to help others get started
- ⬜ Add instructions on how people can secure their backend functionality

## Wish list:
- Would be cool to output the swagger
- As more functionality is provided by server-side SDKs it would be great to have contributions for additional backend functions
- Would be interesting to have a UI that can detect other app services and apply this function app registration as an API permission to those apps so they can request access tokens to this app service when its deployed in Azure.

### Before you get started:
You will need
- Azure Function Core Tools
	https://go.microsoft.com/fwlink/?linkid=2135274
- .NET SDK
	https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60
- An Azure subscription
- Azure Communication Services resource
### Getting set up:
1. rename `local.settings.template.json` to `local.settings.json`

The function app reads from local.settings.json NOT local.settings.template.json. Our git.ignore script avoids local.settings.json which reduces the chance of a leaked key in your repo

2. 
```json
{
    "IsEncrypted": false,
    "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet",
      "AzureCommunicationServicesResourceConnectionString": "The resource connection string in your Azure Communication Services Resource",
      "endpointUrl": "The endpoint URL from your Azure Communication Services resource",
      "adminUserId": "Create one user with chat scope and copy that userId here. (It is helpful for having a server create something on behalf of your end user)"
} 
```
3. `dotnet build`
4. `dotnet run`

## How to try it out:

### Locally:
1. Install [postman](https://www.postman.com/downloads/ "postman")
2. Import our postman collection (/postman/AzureCommunicationServicesGetStartedApis.postman_collection.json)
3. Run the `/api/Identity-CreateUserAndToken` request

_Some of the functions may require additional parameters._

### Deployed on to Azure:

todo

## If you have any additional questions:

Please log any github issues and I can try to get to you when I can.
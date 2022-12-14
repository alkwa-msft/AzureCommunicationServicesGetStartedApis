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
- ✅ Be able to deploy this backend quickly to help others get started
- ✅ Add instructions on how people can secure their backend functionality

## Wish list:
- Would be cool to output the swagger
- As more functionality is provided by server-side SDKs it would be great to have contributions for additional backend functions
- Would be interesting to have a UI that can detect other app services and apply this function app registration as an API permission to those apps so they can request access tokens to this app service when its deployed in Azure.

### How to try it on Azure:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Falkwa-msft%2FAzureCommunicationServicesGetStartedApis%2Fmain%2Fdeploy%2Fazuredeploy.json)

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

This is necessary for the Function App to detect your functions

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

1. Create a Function App on Azure
2. Deploy your function app through VSCode
3. Set up these configuration variables on your function app

todo: link some pages on how to get your resource connection string, endpointUrl and the adminUserId

      "AzureCommunicationServicesResourceConnectionString": "endpoint=<domain>;accesskey=<supertopsecretkey>==",
      "endpointUrl": "<domain>",
      "adminUserId": "<user id you created in your azure portal"

_Try creating your own postman functions to hit your deployed service_

## How to secure your API (Azure Function app) behind an app service

We want to require authentication to get access to our web app and we want to use that same login process to 
get access to our APIs.

1. Add authentication to your Function app. (Select Microsft as your provider)
2. Go into the App registration for the Function app
3. Expose a scope from your Function app (e.g user_impersonation)
4. Add authentication to your App service. (Select Microsoft as your provider)

At this point both of your app service and API are protected but are isolated from one another.

5. Go into the App registration for the App service and add the exposed API URI to the API Permissions of the App registration (App Service)
6. Inside of your web app, use msal-browser to request an access token for the target scope from your azure function app.

MSAL leverages the cookies provided by the authentication step to help us make additional requests for new scopes
(e.g user_impersonation of our API service). When it resolves correctly the audience of the returned access token
will match the audience of the API service. You can use this access token in the Authentication header for the API requests 
(e.g "Bearer `<accessToken>`" There is a space between the r and the beginning of the accessToken)

[Example of using msal-browser in an SPA](https://docs.microsoft.com/en-us/azure/active-directory/develop/tutorial-v2-javascript-auth-code "Example of using msal-browser in an SPA")

It is also possible to get an access token for the other service without using MSAL on the client-side but it is definitely not suggested. I can make a write-up if others are interested. This method will effectively have /.auth/me return an access token for a different audience from the app service audience.

## If you have any additional questions:

Please log any github issues and I can try to get to you when I can.

### if dotnet run is not working:

`npm install -g azure-functions-core-tools@3 --unsafe-perm true`

(In powershell in admin mode)
`Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`

Run this command in your function app root directory
`func start`

- If `func start` is not detecting your functions

you need to create the local.settings.json from the local.settings.template.json 
(make a copy and then rename)
The local.settings.json also tells the app that its a dotnet style function app

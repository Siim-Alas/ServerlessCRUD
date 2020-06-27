# Deployment Instructions

## Code

Get all the projects into VS.

## Storage

Create a new general purpose (v2) Azure **_Storage account_** (from the portal). LRS, hot, and standard performance is enough. Make sure it is private.

Enable the **_Static Website_** function and note the URL.

## Functions

Create a new **_Azure Functions app_** (from the portal), assigning the **_Storage account_** to it. Then you can use the VS 2019 "Publish" dialog to publish (this is a small-scale project), configuring the Storage account as a dependency.

Then, turn on System Managed Identity (in the Identity blade), then go to the **_Storage account_** IAM blade and give the functions app the **_Reader and Data Access_** role.

Lastly, add the **_Blazor UI_** URL to the accepted URLs in CORS settings.

## Blazor UI

Reference: https://anthonychu.ca/post/blazor-azure-storage-static-websites/

Register the app in **_Azure AD_**, then put the appliccable details into ```wwwroot/appsettings.json``` (configure the app in VS). In the Authentication blade, add ```<staticWebsiteBaseUrl>/authentication/login-callback``` to the allowed redirect URLs and enable implicit grant for ID Tokens.

Publish the Blazor project using the default VS "Folder publish" profile. Copy ```ServerlessCrudBlazorUI/bin/Release/netstandard2.1/publish/wwwroot``` to ```$web``` in the **_Storage account_**.

```az storage blob upload-batch --connection-string <connection_string> -s . -d $web```

**You're all done!**
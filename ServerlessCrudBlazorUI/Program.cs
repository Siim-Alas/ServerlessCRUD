using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerlessCrudBlazorUI.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace ServerlessCrudBlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

            builder.Services.AddHttpClient(
                "CrudAPI", 
                client => client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/")
            ).AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            builder.Services.AddTransient<CrudFunctionAPIClient>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    "db944478-cbda-4214-8ad6-7b310465ce97/BlogPosts.Read");
            });

            await builder.Build().RunAsync();
        }
    }
}

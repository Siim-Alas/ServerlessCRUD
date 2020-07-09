using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerlessCrudBlazorUI.Services.APIClients;
using ServerlessCrudBlazorUI.Services.AuthenticationStateProviders;
using ServerlessCrudBlazorUI.Services.HttpMessageHandlers;

namespace ServerlessCrudBlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddScoped<SocialMediaAuthenticationStateProvider>();

            builder.Services.AddTransient<AuthorizedAuthorizationMessageHandler>();
            //builder.Services.AddTransient<AuthenticatedAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<AuthorizedCrudFunctionAPIClient>()
                .AddHttpMessageHandler<AuthorizedAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<AuthenticatedCrudFunctionAPIClient>();
                //.AddHttpMessageHandler<AuthenticatedAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<AnnonymousCrudFunctionAPIClient>();

            builder.Services.AddHttpClient<SocialMediaAccountsAPIClient>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    "db944478-cbda-4214-8ad6-7b310465ce97/BlogPosts.Read");
                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    "db944478-cbda-4214-8ad6-7b310465ce97/Comments.Read");
            });

            await builder.Build().RunAsync();
        }
    }
}

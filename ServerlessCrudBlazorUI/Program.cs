using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerlessCrudBlazorUI.Services.APIClients;
using ServerlessCrudBlazorUI.Services.AuthenticationStateProviders;
using ServerlessCrudBlazorUI.Services.HttpMessageHandlers;
using System.Net.Http;
using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using ServerlessCrudBlazorUI.Services;

namespace ServerlessCrudBlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<SocialMediaAuthenticationStateProvider>();

            builder.Services.AddTransient(options => {
                return new AuthorizedAuthorizationMessageHandler(
                    options.GetRequiredService<IAccessTokenProvider>(),
                    options.GetRequiredService<NavigationManager>()) 
                { 
                    InnerHandler = new HttpClientHandler()
                };
            });
            //builder.Services.AddTransient<AuthenticatedAuthorizationMessageHandler>();

            // DefaultHttpClientFactory DI does not work in Release configuration.
            ////////////////////////////////////////////////////////////////////////////
            
            //builder.Services.AddHttpClient<AuthorizedCrudFunctionAPIClient>()
            //    .AddHttpMessageHandler<AuthorizedAuthorizationMessageHandler>();
            //builder.Services.AddHttpClient<AuthenticatedCrudFunctionAPIClient>();
            //    //.AddHttpMessageHandler<AuthenticatedAuthorizationMessageHandler>();
            //builder.Services.AddHttpClient<AnnonymousCrudFunctionAPIClient>();
            //builder.Services.AddHttpClient<SocialMediaAccountsAPIClient>();

            ////////////////////////////////////////////////////////////////////////////

            builder.Services.AddSingleton(options => {
                return new HttpClient() { BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/") };
            });
            builder.Services.AddScoped<AnnonymousCrudFunctionAPIClient>();
            builder.Services.AddSingleton<AuthenticatedCrudFunctionAPIClient>();
            builder.Services.AddScoped<SocialMediaAccountsAPIClient>();
            builder.Services.AddTransient(options => {
                return new AuthorizedCrudFunctionAPIClient(
                    new HttpClient(options.GetRequiredService<AuthorizedAuthorizationMessageHandler>()));
            });

            builder.Services.AddTransient<BlogPostService>();
            builder.Services.AddSingleton<DocumentEditor>();

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

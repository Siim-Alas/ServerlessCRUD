using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServerlessCrudBlazorUI.Services;

namespace ServerlessCrudBlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient<AuthorizedAuthorizationMessageHandler>();

            builder.Services.AddHttpClient<AuthorizedCrudFunctionAPIClient>()
                .AddHttpMessageHandler<AuthorizedAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<AuthenticatedCrudFunctionAPIClient>()
                .AddHttpMessageHandler<AuthenticatedAuthorizationMessageHandler>();
            builder.Services.AddHttpClient<AnnonymousCrudFunctionAPIClient>();

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

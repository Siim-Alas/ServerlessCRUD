using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ServerlessCrudFunctions.Services;

[assembly: FunctionsStartup(typeof(ServerlessCrudFunctions.Startup))]

namespace ServerlessCrudFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<AADJwtService>();
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerlessCrudFunctions.Services;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(ServerlessCrudFunctions.Startup))]

namespace ServerlessCrudFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<JwtService>();
        }
    }
}

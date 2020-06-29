using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCrudFunctions.Services
{
    public class JwtService
    {
        // Reference: https://github.com/Azure-Samples/ms-identity-dotnet-webapi-azurefunctions

        #region Constants
        private string Audience { get; }
        private string ClientId { get; }
        private string Tenant { get; }
        private string TenantId { get; }

        private string Authority { get { return $"https://login.microsoftonline.com/{Tenant}/v2.0"; } }
        private List<string> ValidIssuers { get { return new List<string>()
            {
                $"https://login.microsoftonline.com/{Tenant}/",
                $"https://login.microsoftonline.com/{Tenant}/v2.0",
                $"https://login.windows.net/{Tenant}/",
                $"https://login.microsoft.com/{Tenant}/",
                $"https://sts.windows.net/{TenantId}/"
            }; } }
        #endregion

        public JwtService()
        {
            Audience = "https://serverlesscrud.azurewebsites.net";
            ClientId = "db944478-cbda-4214-8ad6-7b310465ce97";
            Tenant = "siimalasoutlook.onmicrosoft.com";
            TenantId = "c03a30eb-37d8-4254-914d-a1f7513eba43";
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(HttpRequest request)
        {
            return new JwtSecurityTokenHandler().ValidateToken(
                GetBearerAccessToken(request),
                new TokenValidationParameters()
                {
                    ValidAudiences = new[] { Audience, ClientId },
                    ValidIssuers = ValidIssuers,
                    IssuerSigningKeys = (await GetConfigAsync()).SigningKeys
                },
                out SecurityToken _);
        }

        private string GetBearerAccessToken(HttpRequest request)
        {
            string[] parts = request.Headers["Authorization"].ToString().Split(null);
            if ((parts.Length != 2) || (parts[0] != "Bearer"))
            {
                throw new ArgumentException();
            }
            return parts[1];
        }

        private async Task<OpenIdConnectConfiguration> GetConfigAsync()
        {
            return await new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{Authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever()
                ).GetConfigurationAsync();
        }
    }
}

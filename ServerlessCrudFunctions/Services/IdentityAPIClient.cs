using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessCrudFunctions.Services
{
    public class IdentityAPIClient
    {
        private readonly HttpClient _client;

        public IdentityAPIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<FacebookMeResponse> GetFacebookMeResponseAsync(string accessToken)
        {
            return JsonConvert.DeserializeObject<FacebookMeResponse>(
                    await (await _client.GetAsync($"https://graph.facebook.com/me?access_token={accessToken}")
                    ).Content.ReadAsStringAsync()
                );
        }
    }
}

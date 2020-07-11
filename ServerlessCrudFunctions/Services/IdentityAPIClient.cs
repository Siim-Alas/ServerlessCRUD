using Newtonsoft.Json;
using ServerlessCrudClassLibrary.HttpResponseModels;
using System.Net.Http;
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
        public async Task<GoogleIdTokenResponse> GetGoogleIdTokenResponseAsync(string idToken)
        {
            return JsonConvert.DeserializeObject<GoogleIdTokenResponse>(
                await (await _client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}")
                ).Content.ReadAsStringAsync()
            );
        }
    }
}

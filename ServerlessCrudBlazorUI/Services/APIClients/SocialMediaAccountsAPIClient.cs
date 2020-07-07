using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class SocialMediaAccountsAPIClient
    {
        private readonly HttpClient _client;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _JSRuntime;

        private readonly string callbackAddress = "https://localhost:44389/authentication/login-callback";

        private readonly string facebookBaseAddress = "https://www.facebook.com/v7.0/";
        private readonly string facebookClientID = "201848331183252";
        private readonly string facebookStateString = "a08e09f2eb5deb3319f97a32dbb03b04";

        public SocialMediaAccountsAPIClient(
            HttpClient client, 
            NavigationManager navigationManager, 
            IJSRuntime jSRuntime)
        {
            _client = client;
            _navigationManager = navigationManager;
            _JSRuntime = jSRuntime;
        }

        public string FacebookLoginURL
        {
            get { return $"{facebookBaseAddress}dialog/oauth?client_id={facebookClientID}&redirect_uri={Uri.EscapeDataString(_navigationManager.Uri)}&state={facebookStateString}"; }
        }

        public async Task<string> GetAccessTokenFromFacebook()
        {
            return await _JSRuntime.InvokeAsync<string>("FacebookClient.getAccessToken");
        }
    }
}

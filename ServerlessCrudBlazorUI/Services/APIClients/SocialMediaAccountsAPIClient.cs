using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerlessCrudBlazorUI.Services.JSInteropHelpers;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class SocialMediaAccountsAPIClient : IDisposable
    {
        public delegate void StateHasChangedEventHandler(object source, EventArgs args);
        public event StateHasChangedEventHandler StateHasChanged;

        private readonly HttpClient _client;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _JSRuntime;

        private DotNetObjectReference<CallbackHelper> _callbackReference;

        public SocialMediaAccountsAPIClient(
            HttpClient client, 
            NavigationManager navigationManager, 
            IJSRuntime jSRuntime)
        {
            _client = client;
            _navigationManager = navigationManager;
            _JSRuntime = jSRuntime;
        }
        /// <summary>
        /// Gets or sets the ClaimsPrincipal populated by API calls.
        /// </summary>
        public ClaimsPrincipal User { get; set; }

        #region Facebook
        public static string FacebookAuthenticationType
        {
            get { return "CustomFacebook"; }
        }
        public bool LoggedInWithFacebook 
        { 
            get { return User?.Identity?.AuthenticationType == "CustomFacebook"; }
        }

        public async Task LogInWithFacebook()
        {
            _callbackReference = DotNetObjectReference.Create(new CallbackHelper(FacebookAuthCallback));
            await _JSRuntime.InvokeVoidAsync("FacebookClient.logIn", _callbackReference);
        }
        public async Task LogOutWithFacebook()
        {
            await _JSRuntime.InvokeVoidAsync("FacebookClient.logOut");
            User = null;
            StateHasChanged(this, EventArgs.Empty);
        }
        public async Task FacebookAuthCallback(object[] args)
        {
            FacebookMeResponse response = await _client.GetFromJsonAsync<FacebookMeResponse>($"https://graph.facebook.com/me?access_token={args[1]}");

            User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new Claim("name", response.Name),
                            new Claim("userId", args[0].ToString()), 
                            new Claim("accessToken", args[1].ToString())
                        }, 
                        FacebookAuthenticationType)
                );
            StateHasChanged(this, EventArgs.Empty);
        }
        #endregion

        public void Dispose()
        {
            _callbackReference?.Dispose();
        }
    }
}

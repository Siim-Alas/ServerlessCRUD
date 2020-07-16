using Microsoft.JSInterop;
using ServerlessCrudBlazorUI.Services.AuthenticationStateProviders;
using ServerlessCrudBlazorUI.Services.JSInteropHelpers;
using ServerlessCrudClassLibrary.HttpResponseModels;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class SocialMediaAccountsAPIClient : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IJSRuntime _JSRuntime;
        private readonly SocialMediaAuthenticationStateProvider _socialMediaAuthStateProvider;

        private DotNetObjectReference<VoidCallbackHelper> _callbackReference;

        public SocialMediaAccountsAPIClient(
            HttpClient client, 
            IJSRuntime jSRuntime, 
            SocialMediaAuthenticationStateProvider socialMediaAuthStateProvider)
        {
            _client = client;
            _JSRuntime = jSRuntime;
            _socialMediaAuthStateProvider = socialMediaAuthStateProvider;
        }

        #region Facebook
        public async Task LogInWithFacebook()
        {
            _callbackReference = DotNetObjectReference.Create(new VoidCallbackHelper(FacebookAuthCallback));
            await _JSRuntime.InvokeVoidAsync("FacebookClient.logIn", _callbackReference);
        }
        public async Task LogOutWithFacebook()
        {
            await _JSRuntime.InvokeVoidAsync("FacebookClient.logOut");
            await _socialMediaAuthStateProvider.SignOutUser();
        }
        public async Task FacebookAuthCallback(params object[] args)
        {
            await _socialMediaAuthStateProvider.SignInUser(
                SocialMediaAuthenticationStateProvider.FacebookAuthenticationType, 
                (await _client.GetFromJsonAsync<FacebookMeResponse>($"https://graph.facebook.com/me?access_token={args[1]}"))
                .Name, 
                args[0].ToString(), 
                args[1].ToString());
        }
        #endregion

        #region Google
        public async Task LogInWithGoogle()
        {
            _callbackReference = DotNetObjectReference.Create(new VoidCallbackHelper(GoogleAuthCallback));
            await _JSRuntime.InvokeVoidAsync("GoogleClient.logIn", _callbackReference);
        }
        public async Task LogOutWithGoogle()
        {
            await _JSRuntime.InvokeVoidAsync("GoogleClient.logOut");
            await _socialMediaAuthStateProvider.SignOutUser();
        }
        public async Task GoogleAuthCallback(params object[] args)
        {
            await _socialMediaAuthStateProvider.SignInUser(
                SocialMediaAuthenticationStateProvider.GoogleAuthenticationType, 
                args[0].ToString(), 
                args[1].ToString(), 
                args[2].ToString());
        }
        #endregion

        public async Task LogOut()
        {
            ClaimsPrincipal user = (await _socialMediaAuthStateProvider.GetAuthenticationStateAsync()).User;
            switch (user.Identity.AuthenticationType)
            {
                case SocialMediaAuthenticationStateProvider.FacebookAuthenticationType:
                    await LogOutWithFacebook();
                    break;
                case SocialMediaAuthenticationStateProvider.GoogleAuthenticationType:
                    await LogOutWithGoogle();
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            _callbackReference?.Dispose();
        }
    }
}

﻿using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.AuthenticationStateProviders
{
    public class SocialMediaAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;

        public const string FacebookAuthenticationType = "CustomFacebook";

        public SocialMediaAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(new Claim[] 
                    { 
                        new Claim("name", (await _sessionStorage.GetItemAsync<string>("customAuthStateProviderUserName")) ?? ""),
                        new Claim("userId", (await _sessionStorage.GetItemAsync<string>("customAuthStateProviderUserId")) ?? ""), 
                        new Claim("accessToken", (await _sessionStorage.GetItemAsync<string>("customAuthStateProviderAccessToken")) ?? "")
                    }, 
                    await _sessionStorage.GetItemAsync<string>("customAuthStateProviderAuthType"), 
                    "name", 
                    null)
                )
            );
        }
        public async Task SignInUser(string authenticationType, string userName, string userId, string accessToken)
        {
            await SetLocalStorage(authenticationType, userName, userId, accessToken);

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(
                        new ClaimsPrincipal(
                            new ClaimsIdentity(new Claim[]
                            {
                                new Claim("name", userName),
                                new Claim("userId", userId),
                                new Claim("accessToken", accessToken)
                            },
                            authenticationType,
                            "name",
                            null)
                        )
                    )
                )
            );
        }
        public async Task SignOutUser()
        {
            await SetLocalStorage("", "", "", "");

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }

        private async Task SetLocalStorage(string authenticationType, string userName, string userId, string accessToken)
        {
            await _sessionStorage.SetItemAsync("customAuthStateProviderAuthType", authenticationType);
            await _sessionStorage.SetItemAsync("customAuthStateProviderUserName", userName);
            await _sessionStorage.SetItemAsync("customAuthStateProviderUserId", userId);
            await _sessionStorage.SetItemAsync("customAuthStateProviderAccessToken", accessToken);
        }
    }
}
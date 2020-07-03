﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ServerlessCrudBlazorUI.Services
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(
            IAccessTokenProvider provider,
            NavigationManager navigationManager)
            : base(provider, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { "https://serverlesscrud.azurewebsites.net" },
                scopes: new[] { "db944478-cbda-4214-8ad6-7b310465ce97/BlogPosts.Read" });
        }
    }
}

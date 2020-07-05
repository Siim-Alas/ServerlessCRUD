using System;
using System.Net.Http;

namespace ServerlessCrudBlazorUI.Services
{
    public class AuthenticatedCrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public AuthenticatedCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }
    }
}

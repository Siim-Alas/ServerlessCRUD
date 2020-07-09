using ServerlessCrudClassLibrary;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class AuthenticatedCrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public AuthenticatedCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            //_client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }

        public async Task<HttpResponseMessage> PostCommentEntityAsync(PostCommentEntityRequest request)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "InsertOrMergeComment",
                    request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}

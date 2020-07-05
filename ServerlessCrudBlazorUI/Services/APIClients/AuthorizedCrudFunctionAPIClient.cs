using ServerlessCrudClassLibrary;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class AuthorizedCrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public AuthorizedCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }

        public async Task<HttpResponseMessage> PostBlogPostAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "InsertOrMergeBlogPost",
                    blogPost);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> PostDeleteBlogPostEntityAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "DeleteBlogPost",
                    blogPost);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}

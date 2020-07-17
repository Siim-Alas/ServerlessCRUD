using BlazorInputFile;
using ServerlessCrudClassLibrary.TableEntities;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public async Task<HttpResponseMessage> PostImageAsync(IFileListEntry file)
        {
            try
            {
                StreamContent content = new StreamContent(file.Data);
                content.Headers.ContentType = new MediaTypeHeaderValue(file.Type);
                return await _client.PostAsync($"UploadImage/{file.Name}", content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
        public async Task<string[]> ListImageNamesAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<string[]>("ListImageNames");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new string[0];
            }
        }
    }
}

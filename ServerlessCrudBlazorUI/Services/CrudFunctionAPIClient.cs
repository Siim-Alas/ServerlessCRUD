using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class CrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public CrudFunctionAPIClient(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("CrudAPI");
        }

        public async Task<HttpResponseMessage> PostBlogPostAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "InsertOrMergeBlogPostEntity?code=daYVnhnXf3c4fNZJF3DLbRinnLuI8aAxpw9t2gfb7aVasuCFq6I4RQ==",
                    blogPost);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<ListBlogPostEntitiesRequest> PostListBlogPostsRequestAsync(ListBlogPostEntitiesRequest request)
        {
            try
            {
                Console.WriteLine(_client.DefaultRequestHeaders.Authorization);
                return JsonConvert.DeserializeObject<ListBlogPostEntitiesRequest>(
                    await (await _client.PostAsJsonAsync(
                           "ListBlogPostEntities?code=H90/2vxRzA/kzfaqzhhd9yUCYdFDVJMj//6UedXW8rCgbBX1C6oUSQ==",
                           request
                           )).Content.ReadAsStringAsync()
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return request;
            }
        }

        public async Task<BlogPostEntity> GetBlogPostEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                return JsonConvert.DeserializeObject<BlogPostEntity>(
                    await (await _client.GetAsync(
                        $"ReadBlogPostEntity?partitionkey={partitionKey}&rowkey={rowKey}&code=Dl1wYTdW8GT/DpFhCqK5n2qawDEzg2/teLB3pF4mpZWQNcj6vgsyHA=="
                        )).Content.ReadAsStringAsync()
                    );
            }
            catch
            {
                return new BlogPostEntity("Error", "Blazor Bot", "Something has gone wrong processing your request.");
            }
        }

        public async Task<HttpResponseMessage> PostDeleteBlogPostEntityAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.PostAsJsonAsync(
                    "DeleteBlogPostEntity?code=gnZTJP8y3gxQdpJx6YyTbngYZd3t8ERP5Uy1/iyBW4EOPcigzgpHVQ==",
                    blogPost);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}

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
        private readonly HttpClient _secureClient;
        private readonly HttpClient _annonymousClient;

        public CrudFunctionAPIClient(IHttpClientFactory factory)
        {
            _secureClient = factory.CreateClient("CrudAPI_Secure");
            _annonymousClient = factory.CreateClient("CrudAPI_Annonymous");
        }

        public async Task<HttpResponseMessage> PostBlogPostAsync(BlogPostEntity blogPost)
        {
            try
            {
                return await _secureClient.PostAsJsonAsync(
                    "InsertOrMergeBlogPostEntity",
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
                return JsonConvert.DeserializeObject<ListBlogPostEntitiesRequest>(
                    await (await _annonymousClient.PostAsJsonAsync(
                           "ListBlogPostEntities",
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
                    await (await _annonymousClient.GetAsync(
                        $"ReadBlogPostEntity?partitionkey={partitionKey}&rowkey={rowKey}"
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
                return await _secureClient.PostAsJsonAsync(
                    "DeleteBlogPostEntity",
                    blogPost);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}

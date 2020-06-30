using Newtonsoft.Json;
using ServerlessCrudBlazorUI.Services.Interfaces;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class AnnonymousCrudFunctionAPIClient : IAnnonymousCrudFunctionAPIClient
    {
        protected readonly HttpClient _client;

        public AnnonymousCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }

        public async Task<ListBlogPostEntitiesRequest> PostListBlogPostsRequestAsync(ListBlogPostEntitiesRequest request)
        {
            try
            {
                return JsonConvert.DeserializeObject<ListBlogPostEntitiesRequest>(
                    await (await _client.PostAsJsonAsync(
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
                    await (await _client.GetAsync(
                        $"ReadBlogPostEntity?partitionkey={partitionKey}&rowkey={rowKey}"
                        )).Content.ReadAsStringAsync()
                    );
            }
            catch
            {
                return new BlogPostEntity("Error", "Blazor Bot", "Something has gone wrong processing your request.");
            }
        }
    }
}

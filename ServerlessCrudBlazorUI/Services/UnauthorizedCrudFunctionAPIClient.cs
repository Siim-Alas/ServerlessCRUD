using Newtonsoft.Json;
using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class UnauthorizedCrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public UnauthorizedCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }

        public async Task<QueryBlogPostEntitiesRequest> PostQueryBlogPostsRequestAsync(QueryBlogPostEntitiesRequest request)
        {
            try
            {
                return JsonConvert.DeserializeObject<QueryBlogPostEntitiesRequest>(
                    await (await _client.PostAsJsonAsync(
                           "QueryBlogPostEntities",
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

        public async Task<TableMetadata> GetTableMetadataAsync()
        {
            try
            {
                return JsonConvert.DeserializeObject<TableMetadata>(
                    await (await _client.GetAsync(
                        "GetBlogPostsTableMetadata"
                        )).Content.ReadAsStringAsync()
                    );
            }
            catch
            {
                return new TableMetadata(0);
            }
        }
    }
}

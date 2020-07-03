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

        public async Task<QueryBlogPostEntitiesResponse> GetQueryBlogPostsResponseAsync(
            int skip = 0, 
            string filterString = null,
            IList<string> selectColumns = null,
            int takeCount = 4,
            (string partitionKey, string rowKey)? nextKeys = null)
            {
            try
            {
                // Int default value isn't null so it is assumed to be always included.
                return await _client.GetFromJsonAsync<QueryBlogPostEntitiesResponse>(
                           string.Format(
                               "QueryBlogPostEntities?{0}{1}{2}{3}{4}{5}", 
                               $"skip={skip}",
                               $"&takecount={takeCount}",
                               (filterString == null) ? "" : $"&filterstring={filterString}",
                               (selectColumns == null) ? "" : $"&selectcolumns={JsonConvert.SerializeObject(selectColumns)}",
                               (nextKeys?.partitionKey == null) ? "" : $"&newxtpartitionkey={nextKeys?.partitionKey}",
                               (nextKeys?.rowKey == null) ? "" : $"&nextrowkey={nextKeys?.rowKey}")
                           );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new QueryBlogPostEntitiesResponse(null, new List<BlogPostEntity>());
            }
        }

        public async Task<BlogPostEntity> GetBlogPostEntityAsync(string partitionKey, string rowKey)
        {
            try
            {
                return await _client.GetFromJsonAsync<BlogPostEntity>(
                    $"ReadBlogPostEntity?partitionkey={partitionKey}&rowkey={rowKey}");
            }
            catch
            {
                return new BlogPostEntity("Error", "Blazor Bot", "Something has gone wrong processing your request.");
            }
        }
    }
}

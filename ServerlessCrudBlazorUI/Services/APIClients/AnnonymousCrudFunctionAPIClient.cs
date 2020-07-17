using BlazorInputFile;
using Newtonsoft.Json;
using ServerlessCrudClassLibrary.HttpResponseModels;
using ServerlessCrudClassLibrary.TableEntities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.APIClients
{
    public class AnnonymousCrudFunctionAPIClient
    {
        private readonly HttpClient _client;

        public AnnonymousCrudFunctionAPIClient(HttpClient client)
        {
            _client = client;
            //_client.BaseAddress = new Uri("https://serverlesscrud.azurewebsites.net/api/");
        }

        public async Task<QueryBlogPostEntitiesResponse> GetQueryBlogPostsResponseAsync(
            int skip = 0,
            int takeCount = 4,
            string filterString = null,
            IList<string> selectColumns = null,
            (string partitionKey, string rowKey)? nextKeys = null)
            {
            try
            {
                // Int default value isn't null so it is assumed to be always included.
                return await _client.GetFromJsonAsync<QueryBlogPostEntitiesResponse>(
                           string.Format(
                               "QueryBlogPosts?{0}{1}{2}{3}{4}{5}", 
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
                    $"GetBlogPost?partitionkey={partitionKey}&rowkey={rowKey}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<CommentEntity[]> GetCommentsOnBlogPost(BlogPostEntity blogPost)
        {
            try
            {
                return await _client.GetFromJsonAsync<CommentEntity[]>(
                    $"GetCommentsOnBlogPost?commentpartitionkey={blogPost.PartitionKey}_{blogPost.RowKey}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<TableMetadataEntity> GetTableMetadataAsync(string tableName)
        {
            return await _client.GetFromJsonAsync<TableMetadataEntity>(
                $"GetTableMetadata?tablename={tableName}");
        }
    }
}

using Blazored.LocalStorage;
using Microsoft.Azure.Documents;
using ServerlessCrudBlazorUI.Services.APIClients;
using ServerlessCrudClassLibrary.HttpResponseModels;
using ServerlessCrudClassLibrary.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class BlogPostService
    {
        private readonly AnnonymousCrudFunctionAPIClient _client;
        private readonly ILocalStorageService _localStorage;

        private static readonly int numOfBlogPostsOnPage = 2;
        private static readonly string blogPostsTableName = "blogposts";

        // Local storage keys.
        private static readonly string partitionKeysKey = "partitionKeys";
        private static readonly string rowKeysKey = "rowKeys";
        private static readonly string blogPostsKey = "blogPosts";
        private static readonly string catalogLastQueriedKey = "catalogLastQueried";

        private bool HasNotInitialized { get; set; } = true;

        public BlogPostService(AnnonymousCrudFunctionAPIClient client, ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }
        /// <summary>
        /// The alphabetically sorted PartitionKeys of BlogPostEntities in the storage table.
        /// </summary>
        public string[] PartitionKeys { get; set; }
        /// <summary>
        /// The alphabetically sorted RowKeys of BlogPostEntities in the storage table.
        /// </summary>
        public string[] RowKeys { get; set; }
        public BlogPostEntity[][] BlogPosts { get; set; }
        public DateTime CatalogLastQueried { get; set; }
        public TableMetadataEntity TableMetadata { get; set; }

        public int NumOfPages
        {
            get { return (int)Math.Ceiling(PartitionKeys.Length / (decimal)numOfBlogPostsOnPage); }
        }
        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <returns>A task which initializes the service.</returns>
        public async Task InitAsync()
        {
            CatalogLastQueried = await _localStorage.GetItemAsync<DateTime>(catalogLastQueriedKey);
            TableMetadata = await _client.GetTableMetadataAsync(blogPostsTableName);

            PartitionKeys = await _localStorage.GetItemAsync<string[]>(partitionKeysKey);
            RowKeys = await _localStorage.GetItemAsync<string[]>(rowKeysKey);

            if ((PartitionKeys == null) ||
                (RowKeys == null) ||
                (CatalogLastQueried.Ticks == 0) ||
                (CatalogLastQueried < TableMetadata.Timestamp))
            {
                QueryBlogPostEntitiesResponse response = await _client.GetQueryBlogPostsResponseAsync(
                    takeCount: 1000,
                    selectColumns: new List<string>()
                    {
                        "PartitionKey",
                        "RowKey"
                    });

                PartitionKeys = new string[response.BlogPosts.Count];
                RowKeys = new string[response.BlogPosts.Count];
                for (int i = 0; i < response.BlogPosts.Count; i++)
                {
                    PartitionKeys[i] = response.BlogPosts[i].PartitionKey;
                    RowKeys[i] = response.BlogPosts[i].RowKey;
                }

                await _localStorage.SetItemAsync(catalogLastQueriedKey, DateTime.UtcNow);

                await _localStorage.SetItemAsync(partitionKeysKey, PartitionKeys);
                await _localStorage.SetItemAsync(rowKeysKey, RowKeys);
            }
            else
            {
                BlogPosts = await _localStorage.GetItemAsync<BlogPostEntity[][]>(blogPostsKey);
            }

            if (BlogPosts?.Length != NumOfPages)
            {
                BlogPosts = new BlogPostEntity[NumOfPages][];
            }

            HasNotInitialized = false;
        }
        /// <summary>
        /// Gets the blog posts on a specified page
        /// </summary>
        /// <param name="i">The 0-based index of the page whose blog posts are queried.</param>
        /// <returns>The blog posts on the specified page.</returns>
        public async Task<BlogPostEntity[]> GetBlogPostsOnPageAsync(int i)
        {
            if (HasNotInitialized)
            {
                await InitAsync();
            }

            if (BlogPosts[i] == null)
            {
                BlogPosts[i] = (await _client.GetQueryBlogPostsResponseAsync(
                        selectColumns: new List<string>()
                        {
                            "PartitionKey",
                            "RowKey",
                            "Timestamp",
                            "Author",
                            "Text"
                        },
                        takeCount: numOfBlogPostsOnPage,
                        nextKeys: (PartitionKeys[i * numOfBlogPostsOnPage], RowKeys[i * numOfBlogPostsOnPage])
                    )).BlogPosts.ToArray();

                await _localStorage.SetItemAsync(blogPostsKey, BlogPosts);
            }
            return BlogPosts[i];
        }
        /// <summary>
        /// Gets the blog post with the specified RowKey.
        /// </summary>
        /// <param name="rowKey">The RowKey of the blog post.</param>
        /// <returns>The blog post with the specified RowKey.</returns>
        public async Task<BlogPostEntity> GetBlogPostAsync(string rowKey)
        {
            if (HasNotInitialized)
            {
                await InitAsync();
            }

            int iRk = Array.BinarySearch(RowKeys, rowKey);
            BlogPostEntity[] blogPostsPage = await GetBlogPostsOnPageAsync(iRk / numOfBlogPostsOnPage);
            return blogPostsPage[iRk % numOfBlogPostsOnPage];
        }
        /// <summary>
        /// Gets the blog post on a specified page and index on that page.
        /// </summary>
        /// <param name="page">The 0-based index of the page on which the blog post lies.</param>
        /// <param name="index">The 0-based index of the blog post on the specified page.</param>
        /// <returns>The blog post at the specified page and index on that page.</returns>
        public async Task<BlogPostEntity> GetBlogPostAsync(int page, int index)
        {
            if (HasNotInitialized)
            {
                await InitAsync();
            }

            BlogPostEntity[] blogPostsPage = await GetBlogPostsOnPageAsync(page);
            return blogPostsPage[index];
        }
    }
}

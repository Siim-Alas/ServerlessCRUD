using ServerlessCrudClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services.Interfaces
{
    public interface IAnnonymousCrudFunctionAPIClient
    {
        Task<ListBlogPostEntitiesRequest> PostListBlogPostsRequestAsync(ListBlogPostEntitiesRequest request);
        Task<BlogPostEntity> GetBlogPostEntityAsync(string partitionKey, string rowKey);
    }
}

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class ListBlogPostEntitiesRequest
    {
        public ListBlogPostEntitiesRequest(int skip = 0, 
                                           int takeCount = 4, 
                                           TableContinuationToken continuationToken = null,
                                           List<BlogPostEntity> blogPosts = null)
        {
            Skip = skip;
            TakeCount = takeCount;
            ContinuationToken = continuationToken;
            BlogPosts = blogPosts ?? new List<BlogPostEntity>();
        }
        public int Skip { get; set; }
        public int TakeCount { get; set; }
        public TableContinuationToken ContinuationToken { get; set; }
        public List<BlogPostEntity> BlogPosts { get; set; }
    }
}
